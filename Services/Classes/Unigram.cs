using Services.Interfaces;
using System.Collections.Generic;

namespace Services.Classes
{
    public class Unigram : INgram
    {
        public string Value { get; set; }

        public Unigram(string word)
        {
            Value = word;
        }

        public  List<string> ToList()
        {
            return new List<string>() { Value };
        }

        public string ToSearchTerm()
        {
            return Value;
        }
    }
}
