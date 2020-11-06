namespace DataAccess.Interfaces
{
    public interface IUrlItem
    {
        public int Id { get; set; }
        public string UrlId { get; set; }
        public string Name { get; set; }
        public string UrlName { get; set; }
    }
}
