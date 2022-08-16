using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ImageReference
    {
        public int Id { get; set; }

        [ForeignKey("Media")]
        public int ImageId { get; set; }

        public int ImageSizeType { get; set; }

        public int Builder { get; set; }

        public int HostId { get; set; }

        public int Location { get; set; }

        public virtual Media Media { get; set; }
    }
}
