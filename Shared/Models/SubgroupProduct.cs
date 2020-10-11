using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class SubgroupProduct
    {
        [ForeignKey("Subgroup")]
        public int SubgroupId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        
        public virtual Subgroup Subgroup { get; set; }
        public virtual Product Product { get; set; }
    }
}