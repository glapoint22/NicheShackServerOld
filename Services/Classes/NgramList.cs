using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class NgramList<T> where T : INgram
    {
        public List<T> Ngrams { get; }

        public T Reference { get; }

        public NgramList(List<T> ngrams, T reference)
        {
            Ngrams = ngrams;
            Reference = reference;
        }





        public T GetNgram()
        {
            T ngram;

            if (Ngrams.Count > 1)
            {
                List<string> reference = Reference.ToList();

                ngram = Ngrams
                .OrderByDescending(x => WordLikeness(x.ToList().Aggregate((a, b) => a + b), reference.Aggregate((a, b) => a + b)))
                .FirstOrDefault();
            }
            else
            {
                ngram = Ngrams.FirstOrDefault();
            }
            return ngram;
        }




        private int WordLikeness(string word1, string word2)
        {
            int len = Math.Min(word1.Length, word2.Length);

            int likeness = 0;

            for (int i = 0; i < len; i++)
            {
                if (word1[i] == word2[i])
                {
                    likeness++;
                }
                else
                {
                    likeness--;
                }

            }

            likeness -= Math.Abs(word1.Length - word2.Length);

            return likeness;
        }



    }
}