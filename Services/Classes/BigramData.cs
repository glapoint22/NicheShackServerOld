using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class BigramData : NgramData
    {
        private readonly Dictionary<string, List<string>> wordList;
        private readonly Dictionary<string, List<KeyValuePair<int, int>>> wordIndex;
        private readonly Dictionary<string, Dictionary<string, string>> partialWords;

        public BigramData(Dictionary<string, List<string>> wordList, Dictionary<string, List<KeyValuePair<int, int>>> wordIndex, Dictionary<string, Dictionary<string, string>> partialWords)
        {
            this.wordList = wordList;
            this.wordIndex = wordIndex;
            this.partialWords = partialWords;
        }



        public NgramList<Bigram> GetBigrams(string word1, string word2)
        {
            if (!wordList.ContainsKey(word1)) return null;

            List<Bigram> bigrams = GetWords(word2, wordList[word1], wordIndex[word1])
                .Select(x => new Bigram(word1, x))
                .ToList();

            if (bigrams.Count == 0) return null;

            return new NgramList<Bigram>(bigrams, new Bigram(word1, word2));
        }


        public Bigram GetBigram(Unigram unigram, string partialWord)
        {
            if (!partialWords.ContainsKey(unigram.Value) || !partialWords[unigram.Value].ContainsKey(partialWord)) return null;
            string fullWord = partialWords[unigram.Value][partialWord];

            return new Bigram(unigram.Value, fullWord);
        }




        public NgramList<Bigram> GetBigrams(NgramList<Unigram> unigrams, string referenceWord)
        {
            List<Bigram> bigrams = new List<Bigram>();

            foreach (Unigram unigram in unigrams.Ngrams)
            {
                NgramList<Bigram> bigramList = GetBigrams(unigram.Value, referenceWord);

                if (bigramList != null)
                {
                    bigrams.AddRange(bigramList.Ngrams);
                }
            }

            if (bigrams.Count == 0) return null;

            return new NgramList<Bigram>(bigrams, new Bigram(unigrams.Reference.Value, referenceWord));
        }
    }
}
