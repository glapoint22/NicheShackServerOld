using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class PriceRange
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Label { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
