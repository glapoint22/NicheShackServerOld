using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class SearchTermCorrection
    {
        private readonly UnigramData unigramData;
        private readonly BigramData bigramData;
        private readonly TrigramData trigramData;
        private readonly QuadgramData quadgramData;

        public SearchTermCorrection(Ngrams ngrams)
        {
            unigramData = ngrams.UnigramData;
            bigramData = ngrams.bigramData;
            trigramData = ngrams.trigramData;
            quadgramData = ngrams.quadgramData;
        }



        public string GetCorrectedSearchTerm(string searchTerm)
        {
            string[] wordArray = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // One word
            if (wordArray.Length == 1)
            {
                // Lets try to replace this word
                Unigram unigram = GetUnigram(wordArray[0], out var unigrams);
                if (unigram != null) return unigram.ToSearchTerm();

            }

            // Two words
            else if (wordArray.Length == 2)
            {
                // Lets first try replaing the second word
                Bigram bigram = GetBigram(wordArray[0], wordArray[1], out var bigrams);
                if (bigram != null) return bigram.ToSearchTerm();


                // We were not able to replace the second word. Lets try getting a new first word
                NgramList<Unigram> unigrams;
                Unigram unigram = GetUnigram(wordArray[0], out unigrams);
                if (unigram == null) return null;


                // Using the NEW first word, see if we can replace the second word
                bigram = GetBigram(unigram, wordArray[1]);
                if (bigram != null) return bigram.ToSearchTerm();



                // We were unable to replace the second word. Lets try replacing both words
                bigram = GetBigram(unigrams, wordArray[1], out var bigramsOut);
                if (bigram != null) return bigram.ToSearchTerm();
            }


            // Three words
            else if (wordArray.Length == 3)
            {
                // Lets first try replaing the third word
                Trigram trigram = GetTrigram(wordArray[0], wordArray[1], wordArray[2], out var trigrams);
                if (trigram != null) return trigram.ToSearchTerm();


                // We were unable to replace the third word. Lets try to replace the second word
                NgramList<Bigram> bigrams;
                Bigram bigram = GetBigram(wordArray[0], wordArray[1], out bigrams);


                // If the bigram is null, that means we were unsuccessful in replacing the second word
                if (bigram == null)
                {
                    // Lets try to replace the first word
                    NgramList<Unigram> unigrams = unigramData.GetUnigrams(wordArray[0]);
                    if (unigrams == null) return null;
                    bigram = GetBigram(unigrams, wordArray[1], out bigrams);
                    if (bigram == null) return null;
                }


                // If we made it this far, we were able to get the first two words(bigram).
                // Lets use this bigram to get all three words(our trigram)
                trigram = GetTrigram(bigram, wordArray[2]);
                if (trigram != null) return trigram.ToSearchTerm();



                // If we made it this far, we were not able to replace the third word
                // Lets try to replace all three words
                trigram = GetTrigram(bigrams, wordArray[2], out var trigramsOut);
                if (trigram != null) return trigram.ToSearchTerm();
            }

            // Four or more words
            else if (wordArray.Length >= 4)
            {
                // Lets first try replaing the fourth word
                Quadgram quadgram = GetQuadgram(wordArray[0], wordArray[1], wordArray[2], wordArray[3], out var quadgrams);
                if (quadgram != null)
                {
                    if (wordArray.Length == 4) return quadgram.ToSearchTerm();

                    return getSearchTerm(quadgram, wordArray);
                }

                NgramList<Trigram> trigrams;

                // We were unable to replace the fourth word. Lets try replacing the thrid word
                Trigram trigram = GetTrigram(wordArray[0], wordArray[1], wordArray[2], out trigrams);
                


                // If the trigram is null, that means we were not able to replace the third word
                if (trigram == null)
                {
                    // Lets try replacing the second word
                    NgramList<Bigram> bigrams = bigramData.GetBigrams(wordArray[0], wordArray[1]);

                    // If we have no bigrams, that means we were not able to replace the second word
                    if (bigrams == null)
                    {
                        // Lets try replacing the first word
                        NgramList<Unigram> unigrams = unigramData.GetUnigrams(wordArray[0]);
                        if (unigrams == null) return null;

                        // Lets use our unigrams to get our bigrams
                        bigrams = bigramData.GetBigrams(unigrams, wordArray[1]);
                        if (bigrams == null) return null;
                    }

                    // Now lets use our bigrams to get our trigram
                    trigram = GetTrigram(bigrams, wordArray[2], out trigrams);
                    if (trigram == null) return null;
                }

                // Now lets use our trigram to get the fourth word
                quadgram = GetQuadgram(trigram, wordArray[3]);
                if (quadgram != null)
                {
                    if (wordArray.Length == 4) return quadgram.ToSearchTerm();

                    return getSearchTerm(quadgram, wordArray);
                }



                // We were unable to replace the fourth word.
                // Lets try replacing all four words
                quadgram = GetQuadgram(trigrams, wordArray[3], out var quadgramsOut);
                if (quadgram != null)
                {
                    if (wordArray.Length == 4) return quadgram.ToSearchTerm();

                    return getSearchTerm(quadgram, wordArray);
                }
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
            Bigram bigram = bigramData.GetBigram(unigram, partialWord);
            if (bigram == null) return null;
            return bigram;
        }

        Bigram GetBigram(NgramList<Unigram> unigrams, string word, out NgramList<Bigram> bigrams)
        {
            bigrams = bigramData.GetBigrams(unigrams, word);
            if (bigrams == null) return null;
            return bigrams.GetNgram();
        }

        Trigram GetTrigram(string word1, string word2, string word3, out NgramList<Trigram> trigrams)
        {
            trigrams = trigramData.GetTrigrams(word1, word2, word3);
            if (trigrams == null) return null;
            return trigrams.GetNgram();
        }



        Trigram GetTrigram(Bigram bigram, string partialWord)
        {
            Trigram trigram = trigramData.GetTrigram(bigram, partialWord);
            if (trigram == null) return null;
            return trigram;
        }



        Trigram GetTrigram(NgramList<Bigram> bigrams, string word, out NgramList<Trigram> trigrams)
        {
            trigrams = trigramData.GetTrigrams(bigrams, word);
            if (trigrams == null) return null;
            return trigrams.GetNgram();
        }






        Quadgram GetQuadgram(string word1, string word2, string word3, string word4, out NgramList<Quadgram> quadgrams)
        {
            quadgrams = quadgramData.GetQuadgrams(word1, word2, word3, word4);
            if (quadgrams == null) return null;
            return quadgrams.GetNgram();
        }





        Quadgram GetQuadgram(Trigram trigram, string partialWord)
        {
            Quadgram quadgram = quadgramData.GetQuadgram(trigram, partialWord);
            if (quadgram == null) return null;
            return quadgram;
        }




        Quadgram GetQuadgram(NgramList<Trigram> trigrams, string word, out NgramList<Quadgram> quadgrams)
        {
            quadgrams = quadgramData.GetQuadgrams(trigrams, word);
            if (quadgrams == null) return null;
            return quadgrams.GetNgram();
        }


        string getSearchTerm(Quadgram quadgram, string[] wordArray)
        {
            List<string> wordList = wordArray.Where((x, i) => i > 3).ToList();
            List<string> quadgramList = quadgram.ToList();
            quadgramList.AddRange(wordList);

            string searchTerm = string.Empty;
            quadgramList.ForEach(x => searchTerm += x + " ");

            return searchTerm.TrimEnd();
        }
    }
}
