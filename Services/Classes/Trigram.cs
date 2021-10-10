using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public class Trigram : INgram
    {
        public Tuple<string, string, string> Value { get; set; }


        public Trigram(string word1, string word2, string word3)
        {
            Value = new Tuple<string, string, string>(word1, word2, word3);
        }

        public List<string> ToList()
        {
            return new List<string>() { Value.Item1, Value.Item2, Value.Item3 };
        }

        public string ToSearchTerm()
        {
            return Value.Item1 + " " + Value.Item2 + " " + Value.Item3;
        }
    }
}
