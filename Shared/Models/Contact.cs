using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [MaxLength(20)]
        public string OfficePhone { get; set; }
        [MaxLength(20)]
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Vendor> Vendors { get; set; }

        public Contact()
        {
            Vendors = new HashSet<Vendor>();
        }
    }
}
