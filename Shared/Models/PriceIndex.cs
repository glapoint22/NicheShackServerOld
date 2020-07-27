using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class PriceIndex
    {
        public int Id { get; set; }
        [ForeignKey("ProductContent")]
        [Required]
        public int ProductContentId { get; set; }
        public int Index { get; set; }
        public virtual ProductContent ProductContent { get; set; }
    }
}
