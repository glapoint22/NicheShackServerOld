using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        [ForeignKey("Contact")]
        public int PrimaryContactId { get; set; }
        [ForeignKey("Contact")]
        public int SecondaryContactId { get; set; }
        public string Name { get; set; }
        public string WebPage { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int Zip { get; set; }
        public int PoBox { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public virtual  Contact Contact { get; set; }
    }
}
