using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class Node
    {
        public char Char { get; }
        public Dictionary<char, Node> Children = new Dictionary<char, Node>();
        public Dictionary<string, List<SearchWordSuggestion>> Suggestions = new Dictionary<string, List<SearchWordSuggestion>>();


        public Node() { }

        public Node(char c)
        {
            Char = c;
        }

        public Node(List<SplitSearchTerm> splitSearchTerms, List<string> categoryIds)
        {
            Node rootNode = this;

            for (int i = 0; i < splitSearchTerms.Count; i++)
            {
                Node node = rootNode;

                for (int j = 0; j < splitSearchTerms[i].searchTerm.Length; j++)
                {
                    // This is the current character in the phrase
                    char c = splitSearchTerms[i].searchTerm[j];


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
                    string substring = splitSearchTerms[i].searchTerm.Substring(0, j + 1);




                    // This is where we are adding suggestions based on the substring of the current phrase
                    // ie. ca would get cat, cart, car...
                    for (int k = i; k < splitSearchTerms.Count; k++)
                    {
                        SplitSearchTerm phrase = splitSearchTerms[k];
                        int len = Math.Min(j + 1, phrase.searchTerm.Length);

                        if (phrase.searchTerm.Substring(0, len) == substring)
                        {

                            // List of phrases we are using for suggestions
                            List<SplitSearchTerm> phraseList = new List<SplitSearchTerm>();
                            if (phrase.Parents.Count > 0)
                            {
                                phraseList.AddRange(phrase.Parents.Select(x => new SplitSearchTerm
                                {
                                    searchTerm = x.Name,
                                    Categories = x.Categories,
                                    SearchVolume = x.SearchVolume
                                }));
                            }
                            else
                            {
                                phraseList.Add(phrase);
                            }


                            // Add the phrases to the suggestions list based on category id
                            foreach (SplitSearchTerm currentPhrase in phraseList)
                            {
                                for (int l = 0; l < categoryIds.Count; l++)
                                {
                                    string categoryId = categoryIds[l];


                                    if (l == 0 || currentPhrase.Categories.Select(x => x.UrlId).Contains(categoryId))
                                    {
                                        if (!node.Suggestions.ContainsKey(categoryId)) node.Suggestions.Add(categoryId, new List<SearchWordSuggestion>());
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    var suggestions = node.Suggestions[categoryId];




                                    if (!suggestions.Select(x => x.Name).Contains(currentPhrase.searchTerm))
                                    {
                                        suggestions.Add(new SearchWordSuggestion
                                        {
                                            Name = currentPhrase.searchTerm,
                                            Category = l == 0 && currentPhrase.Categories.Count() > 1 ? currentPhrase.Categories.FirstOrDefault() : null,
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
        }
    }
}