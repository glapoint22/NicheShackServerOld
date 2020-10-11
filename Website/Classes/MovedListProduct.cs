namespace Website.Classes
{
    public struct MovedListProduct
    {
        public int ProductId { get; set; }
        public int CollaboratorId { get; set; }
        public string FromListId { get; set; }
        public string ToListId { get; set; }
    }
}
