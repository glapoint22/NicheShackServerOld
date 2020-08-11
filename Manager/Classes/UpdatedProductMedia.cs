namespace Manager.Classes
{
    public struct UpdatedProductMedia
    {
        public int ProductId { get; set; }
        public int OldMediaId { get; set; }
        public int NewMediaId { get; set; }
    }
}
