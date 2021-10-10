using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class Ngrams
    {
        public UnigramData UnigramData { get; }
        public BigramData bigramData { get; }
        public TrigramData trigramData { get; }


        public Ngrams(List<SplitSearchTerm> splitSearchTerms)
        {
            // Unigrams
            HashSet<string> unigrams = new HashSet<string>();
            List<string> unigramList = new List<string>();
            List<KeyValuePair<int, int>> unigramIndices = new List<KeyValuePair<int, int>>();

            // Bigrams
            Dictionary<string, List<string>> bigramList = new Dictionary<string, List<string>>();
            Dictionary<string, List<KeyValuePair<int, int>>> bigramIndices = new Dictionary<string, List<KeyValuePair<int, int>>>();
            Dictionary<string, Dictionary<string, string>> bigramPartialWords = new Dictionary<string, Dictionary<string, string>>();

            // Trigrams
            Dictionary<Tuple<string, string>, List<string>> trigramList = new Dictionary<Tuple<string, string>, List<string>>();
            Dictionary<Tuple<string, string>, List<KeyValuePair<int, int>>> trigramIndices = new Dictionary<Tuple<string, string>, List<KeyValuePair<int, int>>>();
            Dictionary<Tuple<string, string>, Dictionary<string, string>> trigramPartialWords = new Dictionary<Tuple<string, string>, Dictionary<string, string>>();


            // Quadgrams
            Dictionary<Tuple<string, string, string>, List<string>> quadgramList = new Dictionary<Tuple<string, string, string>, List<string>>();
            Dictionary<Tuple<string, string, string>, List<KeyValuePair<int, int>>> quadgramIndices = new Dictionary<Tuple<string, string, string>, List<KeyValuePair<int, int>>>();
            Dictionary<Tuple<string, string, string>, Dictionary<string, string>> quadgramPartialWords = new Dictionary<Tuple<string, string, string>, Dictionary<string, string>>();


            foreach (SplitSearchTerm splitSearchTerm in splitSearchTerms)
            {
                string[] wordArray = splitSearchTerm.searchTerm.Split();

                for (int i = 0; i < wordArray.Length; i++)
                {
                    if (i == 0)
                    {
                        // Unigrams
                        if (!unigrams.Contains(wordArray[0]))
                        {
                            unigrams.Add(wordArray[0]);
                        }
                    }
                    else if (i == 1)
                    {
                        // Bigrams
                        SetNgramList(bigramList, wordArray[0], wordArray[1]);
                    }
                    else if (i == 2)
                    {
                        // Trigrams
                        SetNgramList(trigramList, new Tuple<string, string>(wordArray[0], wordArray[1]), wordArray[2]);
                    }
                    else if (i == 3)
                    {
                        // Quadgrams
                        SetNgramList(quadgramList, new Tuple<string, string, string>(wordArray[0], wordArray[1], wordArray[2]), wordArray[3]);
                    }
                }
            }



            // Order the lists and create the indices
            unigramList = unigrams.OrderBy(x => x.Length).ToList();
            unigramIndices = unigramList
                .Select((x, i) => new
                {
                    name = x,
                    index = i
                })
                .GroupBy(x => x.name.Length, (key, x) => new KeyValuePair<int, int>(key, x.Select(z => z.index).FirstOrDefault()))
                .ToList();


            bigramList = bigramList.ToDictionary(x => x.Key, x => x.Value.OrderBy(z => z.Count()).ToList());
            bigramIndices = bigramList
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




            trigramList = trigramList.ToDictionary(x => x.Key, x => x.Value.OrderBy(z => z.Count()).ToList());
            trigramIndices = trigramList
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








            quadgramList = quadgramList.ToDictionary(x => x.Key, x => x.Value.OrderBy(z => z.Count()).ToList());
            quadgramIndices = quadgramList
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



            // Generate the partial words
            GeneratePartialWords(bigramList, bigramPartialWords);
            GeneratePartialWords(trigramList, trigramPartialWords);
            GeneratePartialWords(quadgramList, quadgramPartialWords);


            // instantiate the properties
            UnigramData = new UnigramData(unigramList, unigramIndices);
            bigramData = new BigramData(bigramList, bigramIndices, bigramPartialWords);
            trigramData = new TrigramData(trigramList, trigramIndices, trigramPartialWords);
        }


        private void GeneratePartialWords<T>(Dictionary<T, List<string>> ngramList, Dictionary<T, Dictionary<string, string>> ngramPartialWords)
        {
            foreach (KeyValuePair<T, List<string>> ngram in ngramList)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                foreach (string word in ngram.Value)
                {
                    for (int i = 0; i < word.Length; i++)
                    {
                        string substr = word.Substring(0, i + 1);

                        if (!dictionary.ContainsKey(substr))
                        {
                            string newWord = string.Empty;
                            foreach (string currentWord in ngram.Value)
                            {
                                int len = Math.Min(i + 1, currentWord.Length);
                                if (currentWord.Substring(0, len) == substr)
                                {
                                    newWord = currentWord;
                                    break;
                                }
                            }
                            dictionary.Add(substr, newWord);
                        }
                    }
                }
                ngramPartialWords.Add(ngram.Key, dictionary);
            }
        }







        private void SetNgramList<T>(Dictionary<T, List<string>> ngram, T key, string word)
        {
            if (!ngram.ContainsKey(key))
            {
                ngram.Add(key, new List<string> { word });
            }
            else
            {
                if (!ngram[key].Contains(word)) ngram[key].Add(word);
            }
        }
    }
}
