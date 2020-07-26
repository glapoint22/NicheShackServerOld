using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class PriceIndex
    {
        public int Id { get; set; }
        [ForeignKey("ProductContent")]
        [MaxLength(10)]
        public string ProductContentId { get; set; }
        public int Index { get; set; }
        public virtual ProductContent ProductContent { get; set; }
    }
}
