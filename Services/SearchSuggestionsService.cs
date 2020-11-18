using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class SearchSuggestionsService
    {
        public Node root;




        // --------------------------------------------------------------------------------Insert---------------------------------------------------------------
        public void Insert(SearchWord searchWord)
        {
            Node node = root;

            for (int i = 0; i < searchWord.Name.Length; i++)
            {
                char c = searchWord.Name[i];
                if (!node.Children.ContainsKey(c)) node.Children.Add(c, new Node(c));
                node = node.Children[c];
            }

            node.IsWord = true;

            foreach (SearchWordCategory category in searchWord.Categories)
            {
                node.Categories.Add(category.UrlId, category);
            }

            node.SearchVolume = searchWord.SearchVolume;
        }







        // --------------------------------------------------------------------------------Get Suggestions---------------------------------------------------------------
        public List<Suggestion> GetSuggestions(string prefix, string categoryId)
        {
            if (root == null || prefix == null) return null;


            Node node = root;
            int maxSuggestions = 10;


            // Get the current node in the trie by looping through each character in the prefix
            for (int i = 0; i < prefix.Length; i++)
            {
                char c = prefix[i];


                if (node.Children.ContainsKey(c))
                {
                    node = node.Children[c];
                }
                else
                {
                    // No nodes exists for the current character so we need to get a new prefix
                    Prefix newPrefix = new Prefix();

                    if (i == 0)
                    {
                        node = root;
                    }
                    else
                    {
                        node = root.Children[prefix[0]];
                    }

                    // This will get a new prefix based on the current prefix
                    GetNewPrefix(node, prefix, ref newPrefix);

                    prefix = newPrefix.Word;
                    node = newPrefix.Node;

                    break;
                }
            }

            // If we have no prefix, return
            if (prefix == null) return null;



            // This is a queue that will save each character as we traverse through the trie
            // When a word is formed, the word will be saved and the queue item will be removed
            List<QueueItem> queue = new List<QueueItem>();
            queue.Add(new QueueItem
            {
                Node = node,
                Fragments = prefix
            });

            List<SearchWord> searchWords = new List<SearchWord>();


            // Find all matching words
            while (queue.Count > 0)
            {
                // Get the node and fragments from the queue item and then remove the queue item
                QueueItem queueItem = queue[0];
                node = queueItem.Node;
                string fragments = queueItem.Fragments;
                queue.RemoveAt(0);

                // If the fragments form a word
                if (node.IsWord)
                {
                    // If a category id was not passed in or a category id was passed in and the current node has that category
                    if (categoryId == null || node.Categories.ContainsKey(categoryId))
                    {
                        // Add this word to the list of search words
                        searchWords.Add(new SearchWord
                        {
                            Name = fragments,
                            Categories = node.Categories.Select(x => x.Value).ToList(),
                            SearchVolume = node.SearchVolume
                        });
                    }
                }


                // Add all children of this node to the queue
                foreach (var currentNode in node.Children)
                {
                    var child = currentNode.Value;
                    var childWord = fragments + child.Char;

                    queue.Add(new QueueItem
                    {
                        Node = child,
                        Fragments = childWord
                    });
                }
            }


            // Return the suggestions
            return searchWords
                .OrderByDescending(x => x.SearchVolume)
                .Select((x, index) => new Suggestion
                {
                    Name = x.Name,
                    Category = x.Categories.Count > 0 && index == 0 && categoryId == null ? x.Categories
                    .OrderByDescending(z => z.Weight)
                    .Select(z => new SuggestionCategory
                    {
                        Name = z.Name,
                        UrlId = z.UrlId,
                        UrlName = z.UrlName
                    })
                    .FirstOrDefault()
                    : null
                })
                .Take(maxSuggestions)
                .ToList();
        }






        // --------------------------------------------------------------------------------Get New Prefix---------------------------------------------------------------
        private void GetNewPrefix(Node node, string prefix, ref Prefix newPrefix, string word = "")
        {
            float maxEditDistance = 3;

            if (word == string.Empty)
            {
                word = node.Char.ToString();
            }

            foreach (var currentNode in node.Children)
            {
                Node childNode = currentNode.Value;
                string currentWord = word + childNode.Char;

                // Get the edit distance between the prefix and the current word
                float editDistance = GetEditDistance(prefix, currentWord);


                // If the edit distance is less or equal to the max distance 
                // and the current word is a full word
                if (editDistance <= maxEditDistance && childNode.IsWord)
                {
                    // If we don't have a queue item or the current edit distance is less 
                    // than the queueitem edit distance, save a new queue item
                    if (newPrefix.Word == null || editDistance <= newPrefix.EditDistance)
                    {
                        newPrefix = new Prefix
                        {
                            Word = currentWord,
                            Node = childNode,
                            EditDistance = editDistance
                        };
                    }


                }

                GetNewPrefix(childNode, prefix, ref newPrefix, currentWord);
            }
        }














        // --------------------------------------------------------------------------------Get Edit Distance---------------------------------------------------------------
        private float GetEditDistance(string str1, string str2)
        {
            int xLength = str1.Length + 1;
            int yLength = str2.Length + 1;
            int[,] a = new int[xLength, yLength];


            // Poupulate the first row of the grid
            for (int x = 1; x < xLength; x++)
            {
                a[x, 0] = x;
            }


            // Populate the first column of the grid
            for (int y = 1; y < yLength; y++)
            {
                a[0, y] = y;
            }



            // Loop through the grid to compare the two strings
            for (int y = 1; y < yLength; y++)
            {
                for (int x = 1; x < xLength; x++)
                {
                    // Both characters are the same
                    if (str1[x - 1] == str2[y - 1])
                    {
                        a[x, y] = a[x - 1, y - 1];
                    }
                    else
                    {
                        // Perform a replace, insert, or delete operation
                        a[x, y] = Math.Min(Math.Min(a[x - 1, y], a[x - 1, y - 1]), a[x, y - 1]) + 1;
                    }
                }
            }

            float sum = 0;
            for (int i = 1; i < yLength; i++)
            {
                sum += a[xLength - 1, i];
            }

            // Return the avg edit distance
            return sum / (float)(yLength - 1);
        }
    }
}
