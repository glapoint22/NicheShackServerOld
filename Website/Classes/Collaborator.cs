namespace Website.Classes
{
    public struct Collaborator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ListPermissions ListPermissions { get; set; }
        public bool IsRemoved { get; set; }
    }
}