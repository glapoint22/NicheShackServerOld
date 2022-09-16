using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class TempMedia
    {
        public int Id { get; set; }

        [MaxLength(256)]
        public string Name { get; set; }

        [MaxLength(256)]
        public string Url { get; set; }

        [MaxLength(256)]
        public string Thumbnail { get; set; }

        public int Type { get; set; }
    }
}