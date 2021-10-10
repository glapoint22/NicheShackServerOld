using System;

namespace Services.Classes
{
    public class SearchTermCorrection
    {
        private readonly UnigramData unigramData;
        private readonly BigramData bigramData;
        private readonly TrigramData trigramData;

        public SearchTermCorrection(Ngrams ngrams)
        {
            unigramData = ngrams.UnigramData;
            bigramData = ngrams.bigramData;
            trigramData = ngrams.trigramData;
        }



        public string GetCorrectedSearchTerm(string searchTerm)
        {
            string[] wordArray = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (wordArray.Length == 1)
            {
                Unigram unigram = GetUnigram(wordArray[0], out var unigrams);
                if (unigram != null) return unigram.ToSearchTerm();

            }
            else if (wordArray.Length == 2)
            {
                Bigram bigram = GetBigram(wordArray[0], wordArray[1], out var bigrams);
                if (bigram != null) return bigram.ToSearchTerm();



                NgramList<Unigram> unigrams;
                Unigram unigram = GetUnigram(wordArray[0], out unigrams);
                if (unigram == null) return null;

                bigram = GetBigram(unigram, wordArray[1]);
                if (bigram != null) return bigram.ToSearchTerm();




                bigram = GetBigram(unigrams, wordArray[1]);
                if (bigram != null) return bigram.ToSearchTerm();
            }
            else if (wordArray.Length == 3)
            {
                var trigram = GetTrigram(wordArray[0], wordArray[1], wordArray[2], out var trigrams);
                if (trigram != null) return trigram.ToSearchTerm();
            }


            return null;
        }





        Unigram GetUnigram(string word, out NgramList<Unigram> unigrams)
        {
            unigrams = unigramData.GetUnigrams(word);
            if (unigrams == null) return null;

            return unigrams.GetNgram();
        }


        Bigram GetBigram(string word1, string word2, out NgramList<Bigram> bigrams)
        {
            bigrams = bigramData.GetBigrams(word1, word2);
            if (bigrams == null) return null;
            return bigrams.GetNgram();
        }

        Bigram GetBigram(Unigram unigram, string partialWord)
        {
            var bigram = bigramData.GetBigram(unigram.Value, partialWord);
            if (bigram == null) return null;
            return bigram;
        }

        Bigram GetBigram(NgramList<Unigram> unigrams, string word)
        {
            NgramList<Bigram> bigrams = bigramData.GetBigrams(unigrams, word);
            if (bigrams == null) return null;
            return bigrams.GetNgram();
        }

        Trigram GetTrigram(string word1, string word2, string word3, out NgramList<Trigram> trigrams)
        {
            trigrams = trigramData.GetTrigrams(word1, word2, word3);
            if (trigrams == null) return null;
            return trigrams.GetNgram();

        }
    }
}
