using System.Collections.Generic;

namespace Website.Classes
{
    public class Node
    {
        public char Char { get; }
        public bool IsWord { get; set; }
        public Dictionary<char, Node> Children = new Dictionary<char, Node>();
        public Dictionary<string, TrieCategory> Categories = new Dictionary<string, TrieCategory>();
        public float SearchVolume { get; set; }



        public Node() { }

        public Node(char c)
        {
            Char = c;
        }
    }
}