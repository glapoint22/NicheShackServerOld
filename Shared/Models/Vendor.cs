using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Vendor : IItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        [MaxLength(256)]
        public string WebPage { get; set; }
        [MaxLength(256)]
        public string Street { get; set; }
        [MaxLength(256)]
        public string City { get; set; }
        public int? Zip { get; set; }
        public int? PoBox { get; set; }
        [MaxLength(2)]
        public string State { get; set; }
        [MaxLength(256)]
        public string Country { get; set; }
        [MaxLength(256)]
        public string PrimaryFirstName { get; set; }
        [MaxLength(256)]
        public string PrimaryLastName { get; set; }
        [MaxLength(20)]
        public string PrimaryOfficePhone { get; set; }
        [MaxLength(20)]
        public string PrimaryMobilePhone { get; set; }
        [MaxLength(256)]
        public string PrimaryEmail { get; set; }
        [MaxLength(256)]
        public string SecondaryFirstName { get; set; }
        [MaxLength(256)]
        public string SecondaryLastName { get; set; }
        [MaxLength(20)]
        public string SecondaryOfficePhone { get; set; }
        [MaxLength(20)]
        public string SecondaryMobilePhone { get; set; }
        [MaxLength(256)]
        public string SecondaryEmail { get; set; }


        public virtual ICollection<Product> Products { get; set; }


        public Vendor()
        {
            Products = new HashSet<Product>();
        }
    }
}
