using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class SearchTerm
    {
        public string Name { get; set; }
        public List<SuggestionCategory> Categories { get; set; }
        public float SearchVolume { get; set; }

        public static List<SplitSearchTerm> GetSplitSearchTerms(List<SearchTerm> searchTerms)
        {
            List<SplitSearchTerm> splitSearchTerms = new List<SplitSearchTerm>();

            var splitWords = searchTerms
                .Select(x => new
                {
                    WordArray = x.Name.Split(' '),
                    x.Categories,
                    x.SearchVolume,
                    Name = x.Name

                })
                .ToList();





            foreach (var word in splitWords)
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



                    splitSearchTerms.Add(new SplitSearchTerm
                    {
                        searchTerm = phrase,
                        Categories = i == 0 ? word.Categories : null,
                        SearchVolume = i == 0 ? word.SearchVolume : 0,
                        Parents = i > 0 ? new List<SearchTerm>
                        {
                            new SearchTerm {
                                Name = word.Name,
                                Categories = word.Categories,
                                SearchVolume = word.SearchVolume
                            }

                        } : null

                    });
                }
            }

            return splitSearchTerms
                 .GroupBy(x => x.searchTerm, (key, x) => new SplitSearchTerm
                 {
                     searchTerm = key,
                     Categories = x.Select(z => z.Categories).FirstOrDefault(),
                     SearchVolume = x.Select(z => z.SearchVolume).FirstOrDefault(),
                     Parents = x.Where(z => z.Parents != null).Select(z => z.Parents.FirstOrDefault()).ToList()
                 })
                 .OrderBy(x => x.searchTerm)
                 .ToList();
        }
    }
}
