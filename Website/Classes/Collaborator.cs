using System;

namespace Website.Classes
{
    public struct Collaborator
    {
        public string CustomerId { get; set; }
        public string ListId { get; set; }
        public bool IsOwner { get; set; }
        public string Name { get; set; }
    }
}
