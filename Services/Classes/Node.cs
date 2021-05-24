using System.Collections.Generic;

namespace Services.Classes
{
    public class Node
    {
        public char Char { get; }
        public Dictionary<char, Node> Children = new Dictionary<char, Node>();
        public Dictionary<string, List<SearchWordSuggestion>> Suggestions = new Dictionary<string, List<SearchWordSuggestion>>();


        public Node() { }

        public Node(char c)
        {
            Char = c;
        }
    }
}