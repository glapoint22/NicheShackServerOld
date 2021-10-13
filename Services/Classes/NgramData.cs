using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Classes
{
    public class NgramData
    {
        protected List<string> GetWords(string referenceWord, List<string> wordsList, List<KeyValuePair<int, int>> indexList)
        {
            List<string> selectedWords = new List<string>();
            int[] indices = GetStartEndIndices(referenceWord, wordsList, indexList);

            if (indices.Length > 0)
            {
                for (int i = indices[0]; i < indices[1]; i++)
                {
                    var currentWord = wordsList[i];
                    var editDistance = GetEditDistance(referenceWord, currentWord);

                    if (editDistance <= 2)
                    {
                        selectedWords.Add(currentWord);
                    }
                }
            }

            return selectedWords;
        }





        // --------------------------------------------------------------------------------Get Start End Indices---------------------------------------------------------------
        private int[] GetStartEndIndices(string referenceWord, List<string> words, List<KeyValuePair<int, int>> indexList)
        {
            int minLength = Math.Max(referenceWord.Length - 2, 1);
            int maxLength = referenceWord.Length + 3;
            int startIndex = 0;
            int endIndex = 0;



            var indices = indexList
                .Where(x => x.Key >= minLength && x.Key <= maxLength)
                .ToList();


            if (indices.Count == 0 || indices[0].Key >= maxLength)
            {
                return new int[0];
            }

            startIndex = indices[0].Value;


            if (indices[indices.Count - 1].Key < maxLength)
            {
                endIndex = indexList.FirstOrDefault(x => x.Key > maxLength).Value;
            }
            else
            {
                endIndex = indices.SingleOrDefault(x => x.Key == maxLength).Value;
            }


            if (endIndex == 0)
            {
                endIndex = words.Count;
            }

            return new int[] { startIndex, endIndex };
        }









        // --------------------------------------------------------------------------------Get Edit Distance---------------------------------------------------------------
        private float GetEditDistance(string str1, string str2)
        {
            int xLength = str1.Length + 1;
            int yLength = str2.Length + 1;
            int[,] a = new int[xLength, yLength];


            // Poupulate the first row of the grid
            for (int x = 1; x < xLength; x++)
            {
                a[x, 0] = x;
            }


            // Populate the first column of the grid
            for (int y = 1; y < yLength; y++)
            {
                a[0, y] = y;
            }



            // Loop through the grid to compare the two strings
            for (int y = 1; y < yLength; y++)
            {
                for (int x = 1; x < xLength; x++)
                {
                    // Both characters are the same
                    if (str1[x - 1] == str2[y - 1])
                    {
                        a[x, y] = a[x - 1, y - 1];
                    }
                    else
                    {
                        // Perform a replace, insert, or delete operation
                        a[x, y] = Math.Min(Math.Min(a[x - 1, y], a[x - 1, y - 1]), a[x, y - 1]) + 1;
                    }
                }
            }


            return a[str1.Length, str2.Length];
        }
    }
}
