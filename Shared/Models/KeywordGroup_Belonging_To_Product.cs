using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class KeywordGroup_Belonging_To_Product
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }


        [ForeignKey("KeywordGroup")]
        public int KeywordGroupId { get; set; }


        public virtual Product Product { get; set; }
        public virtual KeywordGroup KeywordGroup { get; set; }
    }
}
