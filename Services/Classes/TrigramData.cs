using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class TrigramData : NgramData
    {
        private readonly Dictionary<Tuple<string, string>, List<string>> wordList;
        private readonly Dictionary<Tuple<string, string>, List<KeyValuePair<int, int>>> wordIndex;
        private readonly Dictionary<Tuple<string, string>, Dictionary<string, string>> partialWords;

        public TrigramData(Dictionary<Tuple<string, string>, List<string>> wordList, Dictionary<Tuple<string, string>, List<KeyValuePair<int, int>>> wordIndex, Dictionary<Tuple<string, string>, Dictionary<string, string>> partialWords)
        {
            this.wordList = wordList;
            this.wordIndex = wordIndex;
            this.partialWords = partialWords;
        }




        public NgramList<Trigram> GetTrigrams(string word1, string word2, string word3)
        {
            if (!wordList.ContainsKey(new Tuple<string, string>(word1, word2))) return null;

            List<Trigram> trigrams = GetWords(word3, wordList[new Tuple<string, string>(word1, word2)], wordIndex[new Tuple<string, string>(word1, word2)])
                .Select(x => new Trigram(word1, word2, x))
                .ToList();

            if (trigrams.Count == 0) return null;

            return new NgramList<Trigram>(trigrams, new Trigram(word1, word2, word3));
        }




        public Trigram GetTrigram(Bigram bigram, string partialWord)
        {
            if (!partialWords.ContainsKey(bigram.Value) || !partialWords[bigram.Value].ContainsKey(partialWord)) return null;
            string fullWord = partialWords[bigram.Value][partialWord];

            return new Trigram(bigram.Value.Item1, bigram.Value.Item2, fullWord);
        }




        public NgramList<Trigram> GetTrigrams(NgramList<Bigram> bigrams, string referenceWord)
        {
            List<Trigram> trigrams = new List<Trigram>();

            foreach (Bigram bigram in bigrams.Ngrams)
            {
                NgramList<Trigram> trigramList = GetTrigrams(bigram.Value.Item1, bigram.Value.Item2, referenceWord);

                if (trigramList != null)
                {
                    trigrams.AddRange(trigramList.Ngrams);
                }
            }

            if (trigrams.Count == 0) return null;

            return new NgramList<Trigram>(trigrams, new Trigram(bigrams.Reference.Value.Item1, bigrams.Reference.Value.Item2, referenceWord));
        }
    }
}
