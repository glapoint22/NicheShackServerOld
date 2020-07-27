using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ListCollaborator
    {
        public int Id { get; set; }
        [ForeignKey("Customer")]
        [MaxLength(10)]
        [Required]
        public string CustomerId { get; set; }
        [ForeignKey("List")]
        [MaxLength(10)]
        [Required]
        public string ListId { get; set; }
        public bool IsOwner { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual List List { get; set; }
        public virtual ICollection<ListProduct> ListProducts { get; set; }

        public ListCollaborator()
        {
            ListProducts = new HashSet<ListProduct>();
        }
    }
}