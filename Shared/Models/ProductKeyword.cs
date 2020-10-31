using DataAccess.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ProductKeyword
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        [Required]
        public int ProductId { get; set; }


        [ForeignKey("Keyword")]
        [Required]
        public int KeywordId { get; set; }



        //[Required]
        //[MaxLength(50)]
        //public string Name { get; set; }
        public virtual Product Product { get; set; }
        public virtual Keyword Keyword { get; set; }
    }
}
