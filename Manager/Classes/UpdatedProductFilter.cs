namespace Manager.Classes
{
    public struct UpdatedProductFilter
    {
        public int ProductId { get; set; }
        public int FilterOptionId { get; set; }
        public bool Checked { get; set; }
    }
}
