using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Services
{
    public class SearchSuggestionsService
    {
        public Node root;
        public List<SearchWord> searchWords;


        // --------------------------------------------------------------------------------Insert---------------------------------------------------------------
        public void Insert(SearchWord searchWord, ref Node rootNode, List<SearchWord> words, List<string> categoryIds)
        {

            int index = 0;
            string currentWord = searchWord.Name;
            int wordCount = 0;


            while (index >= 0 && wordCount < 4)
            {
                Node node = rootNode;

                for (int i = 0; i < currentWord.Length; i++)
                {
                    char c = currentWord[i];

                    // If this node existss
                    if (!node.Children.ContainsKey(c))
                    {
                        node.Children.Add(c, new Node(c));

                        node = node.Children[c];


                        Regex regEx;

                        if (wordCount == 0)
                        {
                            regEx = new Regex("^" + currentWord.Substring(0, i + 1));
                        }
                        else
                        {
                            regEx = new Regex("^" + searchWord.Name + "|^" + currentWord.Substring(0, i + 1));
                        }




                        // All Categories
                        node.Suggestions.Add("All", new List<SearchWordSuggestion>());
                        node.Suggestions["All"] = words
                            .OrderByDescending(x => x.SearchVolume)
                            .Where(x => regEx.IsMatch(x.Name))
                            .Select((x, index) => new SearchWordSuggestion
                            {
                                Name = x.Name,
                                Category = index == 0 && x.Categories.Count() > 1 ? x.Categories.FirstOrDefault() : null,
                                SearchVolume = x.SearchVolume
                            })
                            .Take(10)
                            .ToList();

                        foreach (string categoryId in categoryIds)
                        {
                            // Current category
                            node.Suggestions.Add(categoryId, new List<SearchWordSuggestion>());
                            node.Suggestions[categoryId] = words
                                .OrderByDescending(x => x.SearchVolume)
                                .Where(x => regEx.IsMatch(x.Name))
                                .Where(x => x.Categories.Select(z => z.UrlId).ToList().Contains(categoryId))
                                .Select((x, index) => new SearchWordSuggestion
                                {
                                    Name = x.Name,
                                    SearchVolume = x.SearchVolume
                                })
                                .Take(10)
                                .ToList();
                        }
                    }

                    // Node does exist
                    else
                    {
                        node = node.Children[c];

                        // Word count within the current search word is greater than 0
                        if (wordCount > 0)
                        {

                            // Get the current search word
                            List<SearchWordSuggestion> currentSearchWords = words
                            .Where(x => x.Name == searchWord.Name)
                            .Select(x => new SearchWordSuggestion
                            {
                                Name = x.Name,
                                SearchVolume = x.SearchVolume,
                                Category = x.Categories.Count() > 1 ? x.Categories.FirstOrDefault() : null,
                            })
                            .ToList();


                            // Concat the current search word with all other suggestions
                            node.Suggestions["All"] = currentSearchWords.Concat(node.Suggestions["All"])

                                .Select(x => new
                                {
                                    Name = x.Name,
                                    Category = x.Category,
                                    SearchVolume = x.SearchVolume
                                })
                                .OrderByDescending(x => x.SearchVolume)
                                .Distinct()
                                .Select((x, index) => new SearchWordSuggestion
                                {
                                    Name = x.Name,
                                    Category = index == 0 && x.Category != null ? x.Category : null,
                                    SearchVolume = x.SearchVolume
                                })
                                .Take(10)
                                .ToList();


                            // Each category
                            foreach (string categoryId in categoryIds)
                            {
                                // Get the current search word
                                currentSearchWords = words
                                .Where(x => x.Name == searchWord.Name)
                                .Where(x => x.Categories.Select(z => z.UrlId).ToList().Contains(categoryId))
                                .Select(x => new SearchWordSuggestion
                                {
                                    Name = x.Name,
                                    SearchVolume = x.SearchVolume,
                                })
                                .ToList();


                                if (currentSearchWords.Count() > 0)
                                {
                                    // Concat the current search word with all other suggestions
                                    node.Suggestions[categoryId] = currentSearchWords.Concat(node.Suggestions[categoryId])
                                    .Select((x, index) => new
                                    {
                                        Name = x.Name,
                                        SearchVolume = x.SearchVolume
                                    })
                                    .OrderByDescending(x => x.SearchVolume)
                                    .Distinct()
                                    .Select((x, index) => new SearchWordSuggestion
                                    {
                                        Name = x.Name,
                                        SearchVolume = x.SearchVolume
                                    })
                                    .Take(10)
                                    .ToList();
                                }
                            }
                        }
                    }
                }


                index = searchWord.Name.IndexOf(" ", index);
                if (index != -1)
                {
                    currentWord = searchWord.Name.Substring(index + 1);
                    index++;
                    wordCount++;
                }
            }


        }







        // --------------------------------------------------------------------------------Get Suggestions---------------------------------------------------------------
        public List<Suggestion> GetSuggestions(string prefix, string categoryId)
        {
            if (root == null || prefix == null) return null;


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
                    if (i == 0) prefix = null;


                    // This will get a new prefix based on the first character of the current prefix
                    if (i > 0) prefix = GetNewPrefix(prefix[0], prefix);

                    break;
                }
            }

            // If we have no prefix, return
            if (prefix == null) return null;

            if (categoryId == null) categoryId = "All";

            if (node.Suggestions[categoryId].Count > 0 && node.Suggestions[categoryId][0].Category != null) node.Suggestions[categoryId].Insert(0, new SearchWordSuggestion { Name = node.Suggestions[categoryId][0].Name });
            return node.Suggestions[categoryId]
                .Select(x => new Suggestion
                {
                    Name = x.Name,
                    Category = x.Category
                })
                .ToList();
        }






        // --------------------------------------------------------------------------------Get New Prefix---------------------------------------------------------------
        private string GetNewPrefix(char character, string prefix)
        {
            Regex regEx = new Regex("^" + character);

            return searchWords
                .Where(x => regEx.IsMatch(x.Name))
                .Where(x => GetEditDistance(prefix, x.Name) == 1)
                .Select(x => x.Name)
                .FirstOrDefault();
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


            return a[str1.Length, str2.Length];
        }
    }
}
