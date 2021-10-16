using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class NgramList<T> where T : class, INgram
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
            List<string> reference = Reference.ToList();

            var result = Ngrams.Select(x => new
            {
                ngram = x,
                likeness = WordLikeness(x.ToList().Aggregate((a, b) => a + b), reference.Aggregate((a, b) => a + b))
            })
                .OrderByDescending(x => x.likeness)
                .FirstOrDefault();

            if (result.likeness <= 0) return null;
            return result.ngram;
        }




        private int WordLikeness(string word, string reference)
        {
            int likeness = 0;
            int startIndex = 0;
            int referenceStartIndex = 0;
            int maxLength = Math.Max(word.Length, reference.Length);

            word = word.PadRight(maxLength);
            reference = reference.PadRight(maxLength);


            for (int i = 0; i < word.Length; i++)
            {
                int length = (i + 1) - startIndex;
                string substring = word.Substring(startIndex, length);

                if (reference.IndexOf(substring, referenceStartIndex) != -1)
                {
                    likeness++;
                }
                else
                {
                    likeness--;
                    startIndex = i + 1;
                    referenceStartIndex = i;
                }
            }

            return likeness;
        }
    }
}