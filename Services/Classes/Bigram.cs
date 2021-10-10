using Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Services.Classes
{
    public class Bigram : INgram
    {
        public Tuple<string, string> Value { get; set; }


        public Bigram(string word1, string word2)
        {
            Value = new Tuple<string, string>(word1, word2);
        }

        public List<string> ToList()
        {
            return new List<string>() { Value.Item1, Value.Item2 };
        }
        

        public string ToSearchTerm()
        {
            return Value.Item1 + " " + Value.Item2;
        }
    }
}
