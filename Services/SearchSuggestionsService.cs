using Services.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Services
{
    public class SearchSuggestionsService
    {
        public Node rootNode;
        public SearchTermCorrection searchTermCorrection;


        // --------------------------------------------------------------------------------Get Suggestions---------------------------------------------------------------
        public List<Suggestion> GetSuggestions(string searchTerm, string categoryId)
        {
            if (rootNode == null || searchTerm == null) return null;


            Node node = rootNode;
            Regex regex = new Regex(@"[\s]{2,}");

            // Remove unwanted spaces
            searchTerm = searchTerm.TrimStart();
            searchTerm = regex.Replace(searchTerm, " ");


            bool searchTermCorrected = false;

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
                    if (searchTermCorrected) return null;
                    searchTerm = searchTermCorrection.GetCorrectedSearchTerm(searchTerm);
                    searchTermCorrected = true;

                    if (searchTerm == null) return null;

                    node = rootNode;
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
    }
}