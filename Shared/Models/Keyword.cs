using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Keyword
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public string ProductId { get; set; }
        public string Name { get; set; }
        public virtual Product Product { get; set; }
    }
}
