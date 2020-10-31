using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels;

namespace Website.Classes
{
    public class Trie
    {
        private Node root;

        public Trie()
        {
            root = new Node();
        }




        public void Insert(string word, TrieCategory category, float searchVolume)
        {
            Node node = root;

            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                if (!node.Children.ContainsKey(c)) node.Children.Add(c, new Node(c));
                node = node.Children[c];
            }

            node.IsWord = true;
            if (!node.Categories.ContainsKey(category.UrlId)) node.Categories.Add(category.UrlId, category);

            node.SearchVolume = searchVolume;
        }








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





        private void GetNewPrefix(Node node, string prefix, ref NewPrefix newPrefix, string word = "")
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
                        newPrefix = new NewPrefix
                        {
                            Word = currentWord,
                            Node = childNode,
                            EditDistance = editDistance
                        };

                        // If edit distance is 1, break;
                        //if (editDistance == 1) break;
                    }


                }

                GetNewPrefix(childNode, prefix, ref newPrefix, currentWord);

                // If edit distance is 1, break;
                //if (newPrefix.EditDistance == 1) break;
            }
        }







        public List<Suggestion> GetSuggestions(string prefix, string categoryId)
        {
            if (prefix == null) return null;


            Node node = root;


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
                    NewPrefix newPrefix = new NewPrefix();

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

            List<TrieWord> words = new List<TrieWord>();


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
                        TrieCategory category = null;

                        // If we are not passing in a category id and this node has more than one category
                        // Use the node with the highest sales count
                        if (node.Categories.Count > 1 && categoryId == null)
                        {
                            category = node.Categories
                            .OrderByDescending(x => x.Value.SalesCount)
                            .Select(x => x.Value)
                            .FirstOrDefault();
                        }

                        
                        words.Add(new TrieWord
                        {
                            Name = fragments,
                            Category = category,
                            SearchVolume = node.SearchVolume
                        });
                    }
                }



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



            return words
                .OrderByDescending(x => x.SearchVolume)
                .Select((x, index) => new Suggestion
                {
                    Name = x.Name,
                    Category = x.Category != null && index == 0 ? new SuggestionCategory
                    {
                        Name = x.Category.Name,
                        UrlName = x.Category.UrlName,
                        UrlId = x.Category.UrlId
                    }
                    : null
                })
                .Take(10)
                .ToList();
        }
    }



    struct QueueItem
    {
        public Node Node { get; set; }
        public string Fragments { get; set; }
    }

    struct NewPrefix
    {
        public Node Node { get; set; }
        public string Word { get; set; }
        public float EditDistance { get; set; }
    }
}
