using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ImageReference
    {
        public int Id { get; set; }

        [ForeignKey("Media")]
        public int ImageId { get; set; }

        public int ImageSize { get; set; }

        public int Builder { get; set; }

        [MaxLength(256)]
        [Required]
        public string Host { get; set; }

        public int Location { get; set; }

        public virtual Media Media { get; set; }
    }
}
