using Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Services.Classes
{
    public class Quadgram : INgram
    {
        public Tuple<string, string, string, string> Value { get; set; }

        public Quadgram(string word1, string word2, string word3, string word4)
        {
            Value = new Tuple<string, string, string, string>(word1, word2, word3, word4);
        }

        public List<string> ToList()
        {
            return new List<string>() { Value.Item1, Value.Item2, Value.Item3, Value.Item4 };
        }

        public string ToSearchTerm()
        {
            return Value.Item1 + " " + Value.Item2 + " " + Value.Item3 + " " + Value.Item4;
        }
    }
}