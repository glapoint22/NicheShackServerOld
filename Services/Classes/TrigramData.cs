using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
