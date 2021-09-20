using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Services
{
    public class SearchSuggestionsService
    {
        Node root;

        public void Init(List<int> productOrderIds, List<KeywordInfo> keywords, List<string> categoryIds)
        {
            List<SearchWord> searchWords = GetSearchWords(keywords, productOrderIds);
            Node rootNode = new Node();
            List<Phrase> phrases = GetPhrases(searchWords);
            categoryIds.Insert(0, "All");


            Dictionary<string, List<string>> bigramsList = new Dictionary<string, List<string>>();
            Dictionary<string, List<KeyValuePair<int, int>>> bigramIndices = new Dictionary<string, List<KeyValuePair<int, int>>>();
            Dictionary<KeyValuePair<string, string>, double> bigrams = new Dictionary<KeyValuePair<string, string>, double>();
            Dictionary<string, List<string>> splitWords = new Dictionary<string, List<string>>();

            GetData(phrases, categoryIds, ref rootNode, ref bigramsList, ref bigramIndices, ref bigrams, ref splitWords);




            root = rootNode;
        }





        void GetData(List<Phrase> phrases, List<string> categoryIds, ref Node rootNode, ref Dictionary<string, List<string>> bigramsList, ref Dictionary<string, List<KeyValuePair<int, int>>> bigramIndices, ref Dictionary<KeyValuePair<string, string>, double> bigrams, ref Dictionary<string, List<string>> splitWords)
        {
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            Dictionary<KeyValuePair<string, string>, int> bigramCount = new Dictionary<KeyValuePair<string, string>, int>();

            for (int i = 0; i < phrases.Count; i++)
            {
                SetNodes(phrases, i, categoryIds, ref rootNode);
                SetData(phrases, i, ref splitWords, ref wordCount, ref bigramsList, ref bigramCount);
            }


            bigramsList = bigramsList.ToDictionary(x => x.Key, x => x.Value.OrderBy(z => z.Count()).ToList());
            bigrams = bigramCount.ToDictionary(x => x.Key, x => Math.Log((double)x.Value / (double)wordCount[x.Key.Key]));
            bigramIndices = bigramsList
                .Select(x => new
                {
                    x.Key,
                    value = x.Value.Select((z, i) => new
                    {
                        name = z,
                        index = i
                    })
                    .GroupBy(z => z.name.Length, (key, z) => new KeyValuePair<int, int>(key, z.Select(y => y.index).FirstOrDefault()))
                    .ToList()

                }).ToDictionary(x => x.Key, x => x.value);
        }


        List<SearchWord> GetSearchWords(List<KeywordInfo> keywords, List<int> productOrderIds)
        {
            return keywords.GroupBy(x => x.Name, (key, x) => new
            {
                name = key,
                searchVolume = x.Select(z => z.SearchVolume).FirstOrDefault(),
                categories = x.Select(z => new
                {
                    z.Category.Name,
                    z.Category.UrlId,
                    z.Category.UrlName,
                    productIds = z.Products.Where(w => w.CategoryId == z.Category.UrlId)
                    .Select(a => new
                    {
                        a.Id,
                        a.Rating,
                        a.MediaCount,
                        salesCount = productOrderIds.Count(c => c == a.Id)
                    })
                    .ToList()
                })
                    .GroupBy(x => x.UrlId, (key, a) => new
                    {
                        UrlId = key,
                        Name = a.Select(w => w.Name).FirstOrDefault(),
                        UrlName = a.Select(w => w.UrlName).FirstOrDefault(),
                        productIds = a.Select(w => w.productIds).FirstOrDefault()
                    })
                    .Select(z => new
                    {
                        z.UrlId,
                        z.Name,
                        z.UrlName,
                        salesCount = z.productIds.Select(w => w.salesCount).Sum(),
                        mediaCount = z.productIds.Select(w => w.MediaCount).Sum(),
                        rating = z.productIds.Select(w => w.Rating).Sum() / z.productIds.Count()
                    })
                    .Select(z => new
                    {
                        UrlId = z.UrlId,
                        Name = z.Name,
                        UrlName = z.UrlName,
                        Weight = (z.rating * 0.8) + (z.salesCount * 0.15) + (z.mediaCount * .05)
                    }).ToList()
            })
                .Select(x => new SearchWord
                {
                    Name = x.name,
                    SearchVolume = x.searchVolume,
                    Categories = x.categories
                        .OrderByDescending(z => z.Weight)
                        .Select(z => new SuggestionCategory
                        {
                            UrlId = z.UrlId,
                            Name = z.Name,
                            UrlName = z.UrlName
                        })
                        .ToList()
                })
                .ToList();
        }









        







        // --------------------------------------------------------------------------------Get Suggestions---------------------------------------------------------------
        public List<Suggestion> GetSuggestions(string searchTerm, string categoryId)
        {
            if (root == null || searchTerm == null) return null;


            Node node = root;
            Regex regex = new Regex(@"[\s]{2,}");

            // Remove unwanted spaces
            searchTerm = searchTerm.TrimStart();
            searchTerm = regex.Replace(searchTerm, " ");



            // Get the current node in the trie by looping through each character in the search term
            for (int i = 0; i < searchTerm.Length; i++)
            {
                char c = searchTerm[i];


                if (node.Children.ContainsKey(c))
                {
                    node = node.Children[c];
                }

                // No nodes exists for the current character
                else
                {


                    node = root;
                    i = -1;
                }
            }

            if (categoryId == null) categoryId = "All";

            if (!node.Suggestions.ContainsKey(categoryId)) return null;

            var suggestions = node.Suggestions[categoryId]
                .Select(x => new Suggestion
                {
                    Name = x.Name,
                    Category = x.Category
                })
                .ToList();

            if (categoryId == "All" && suggestions[0].Category != null) suggestions.Insert(0, new Suggestion { Name = suggestions[0].Name });

            return suggestions;
        }





        List<Phrase> GetPhrases(List<SearchWord> searchWords)
        {
            List<Phrase> phrases = new List<Phrase>();

            var searchWordSplit = searchWords
                .Select(x => new
                {
                    WordArray = x.Name.Split(' '),
                    x.Categories,
                    x.SearchVolume,
                    Name = x.Name

                })
                .ToList();





            foreach (var word in searchWordSplit)
            {
                for (int i = 0; i < word.WordArray.Length; i++)
                {
                    if (i > 3) break;
                    string phrase = string.Empty;
                    for (int j = i; j < word.WordArray.Length; j++)
                    {
                        phrase += word.WordArray[j] + " ";
                    }

                    phrase = phrase.Trim();



                    phrases.Add(new Phrase
                    {
                        Name = phrase,
                        Categories = i == 0 ? word.Categories : null,
                        SearchVolume = i == 0 ? word.SearchVolume : 0,
                        Parents = i > 0 ? new List<SearchWord>
                        {
                            new SearchWord {
                                Name = word.Name,
                                Categories = word.Categories,
                                SearchVolume = word.SearchVolume
                            }

                        } : null

                    });
                }
            }

            return phrases
                 .GroupBy(x => x.Name, (key, x) => new Phrase
                 {
                     Name = key,
                     Categories = x.Select(z => z.Categories).FirstOrDefault(),
                     SearchVolume = x.Select(z => z.SearchVolume).FirstOrDefault(),
                     Parents = x.Where(z => z.Parents != null).Select(z => z.Parents.FirstOrDefault()).ToList()
                 })
                 .OrderBy(x => x.Name)
                 .ToList();
        }







        void SetNodes(List<Phrase> phrases, int index, List<string> categoryIds, ref Node rootNode)
        {
            Node node = rootNode;

            for (int i = 0; i < phrases[index].Name.Length; i++)
            {
                // This is the current character in the phrase
                char c = phrases[index].Name[i];


                // If this node existss
                if (!node.Children.ContainsKey(c))
                {
                    node.Children.Add(c, new Node(c));
                }
                else
                {
                    node = node.Children[c];
                    continue;
                }

                node = node.Children[c];
                string substring = phrases[index].Name.Substring(0, i + 1);




                // This is where we are adding suggestions based on the substring of the current phrase
                // ie. ca would get cat, cart, car...
                for (int j = index; j < phrases.Count; j++)
                {
                    Phrase phrase = phrases[j];
                    int len = Math.Min(i + 1, phrase.Name.Length);

                    if (phrase.Name.Substring(0, len) == substring)
                    {

                        // List of phrases we are using for suggestions
                        List<Phrase> phraseList = new List<Phrase>();
                        if (phrase.Parents.Count > 0)
                        {
                            phraseList.AddRange(phrase.Parents.Select(x => new Phrase
                            {
                                Name = x.Name,
                                Categories = x.Categories,
                                SearchVolume = x.SearchVolume
                            }));
                        }
                        else
                        {
                            phraseList.Add(phrase);
                        }


                        // Add the phrases to the suggestions list based on category id
                        foreach (Phrase currentPhrase in phraseList)
                        {
                            for (int k = 0; k < categoryIds.Count; k++)
                            {
                                string categoryId = categoryIds[k];


                                if (k == 0 || currentPhrase.Categories.Select(x => x.UrlId).Contains(categoryId))
                                {
                                    if (!node.Suggestions.ContainsKey(categoryId)) node.Suggestions.Add(categoryId, new List<SearchWordSuggestion>());
                                }
                                else
                                {
                                    continue;
                                }

                                var suggestions = node.Suggestions[categoryId];




                                if (!suggestions.Select(x => x.Name).Contains(currentPhrase.Name))
                                {
                                    suggestions.Add(new SearchWordSuggestion
                                    {
                                        Name = currentPhrase.Name,
                                        Category = k == 0 && currentPhrase.Categories.Count() > 1 ? currentPhrase.Categories.FirstOrDefault() : null,
                                        SearchVolume = currentPhrase.SearchVolume
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }



                // This will order the suggestions by search volume and limit the number of suggestions to 10
                node.Suggestions = node.Suggestions
                    .ToDictionary(x => x.Key, x => x.Value
                        .OrderByDescending(z => z.SearchVolume)
                        .Select((z, i) => new SearchWordSuggestion
                        {
                            Name = z.Name,
                            Category = (i == 0 ? z.Category : null),
                            SearchVolume = z.SearchVolume
                        })
                        .Take(10)
                        .ToList());
            }
        }





        void GenerateSplitWords(string word, int index, List<Phrase> phrases, ref Dictionary<string, List<string>> words)
        {
            for (int j = 0; j < word.Length; j++)
            {
                string substring = word.Substring(0, j + 1);
                List<string> wordList;

                if (!words.ContainsKey(substring))
                {
                    wordList = new List<string>();
                    words.Add(substring, wordList);
                }
                else
                {
                    continue;
                }



                for (int k = index; k < phrases.Count; k++)
                {
                    int len = Math.Min(j + 1, phrases[k].Name.Length);

                    if (phrases[k].Name.Substring(0, len) == substring)
                    {
                        int idx = phrases[k].Name.IndexOf(' ');

                        if (idx == -1) idx = phrases[k].Name.Length;

                        string splitWord = phrases[k].Name.Substring(0, idx);

                        if (!wordList.Contains(splitWord))
                        {
                            wordList.Add(splitWord);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }




        void GenerateWordCount(string firstWord, string secondWord, bool endOfPhrase, ref Dictionary<string, int> wordCount)
        {
            if (!wordCount.ContainsKey(firstWord))
            {
                wordCount.Add(firstWord, 1);
            }
            else
            {
                wordCount[firstWord] += 1;
            }



            if (endOfPhrase)
            {
                if (!wordCount.ContainsKey(secondWord))
                {
                    wordCount.Add(secondWord, 1);
                }
                else
                {
                    wordCount[secondWord] += 1;
                }
            }
        }


        void GenerateBigramsCount(string firstWord, string secondWord, ref Dictionary<KeyValuePair<string, string>, int> bigramCount)
        {
            var key = new KeyValuePair<string, string>(firstWord, secondWord);
            if (!bigramCount.ContainsKey(key))
            {
                bigramCount.Add(key, 1);
            }
            else
            {
                bigramCount[key] += 1;
            }
        }



        void GenerateBigramsList(string firstWord, string secondWord, ref Dictionary<string, List<string>> bigramsList)
        {
            if (!bigramsList.ContainsKey(firstWord))
            {
                bigramsList.Add(firstWord, new List<string> { secondWord });
            }
            else
            {
                if (!bigramsList[firstWord].Contains(secondWord)) bigramsList[firstWord].Add(secondWord);
            }
        }


        void SetData(List<Phrase> phrases, int index, ref Dictionary<string, List<string>> splitWords, ref Dictionary<string, int> wordCount, ref Dictionary<string, List<string>> bigramsList, ref Dictionary<KeyValuePair<string, string>, int> bigramCount)
        {
            string phrase = phrases[index].Name;
            string[] wordArray = phrase.Split();

            for (int i = 0; i < wordArray.Length; i++)
            {
                string firstWord;
                string secondWord = wordArray[i];

                if (i == 0)
                {
                    firstWord = "<start>";
                    GenerateSplitWords(secondWord, index, phrases, ref splitWords);
                }

                else
                {
                    firstWord = wordArray[i - 1];
                }


                GenerateWordCount(firstWord, secondWord, i == wordArray.Length - 1, ref wordCount);
                GenerateBigramsCount(firstWord, secondWord, ref bigramCount);
                GenerateBigramsList(firstWord, secondWord, ref bigramsList);
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


            return a[str1.Length, str2.Length];
        }
    }

    public class Phrase
    {
        public string Name { get; set; }
        public List<SuggestionCategory> Categories { get; set; }
        public float SearchVolume { get; set; }
        public List<SearchWord> Parents { get; set; }

    }
}
