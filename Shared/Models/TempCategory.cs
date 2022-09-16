using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class TempCategory
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string UrlId { get; set; }

        [Required]
        [MaxLength(256)]
        public string UrlName { get; set; }

        public int? ImageId { get; set; }


        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}