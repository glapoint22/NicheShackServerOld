using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class UnigramData : NgramData
    {
        private readonly List<string> wordList;
        private readonly List<KeyValuePair<int, int>> wordIndex;

        public UnigramData(List<string> wordList, List<KeyValuePair<int, int>> wordIndex)
        {
            this.wordList = wordList;
            this.wordIndex = wordIndex;
        }

        public NgramList<Unigram> GetUnigrams(string word)
        {
            List<Unigram> unigrams = GetWords(word, wordList, wordIndex)
                .Select(x => new Unigram(x))
                .ToList();

            if (unigrams.Count == 0) return null;

            return new NgramList<Unigram>(unigrams, new Unigram(word));
        }
    }
}
