using System.Collections.Generic;

namespace Services.Classes
{
    public class Node
    {
        public char Char { get; }
        public bool IsWord { get; set; }
        public Dictionary<char, Node> Children = new Dictionary<char, Node>();
        public Dictionary<string, SearchWordCategory> Categories = new Dictionary<string, SearchWordCategory>();
        public float SearchVolume { get; set; }



        public Node() { }

        public Node(char c)
        {
            Char = c;
        }
    }
}