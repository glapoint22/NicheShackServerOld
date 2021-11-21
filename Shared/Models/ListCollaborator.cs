using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ListCollaborator
    {
        public int Id { get; set; }
        [ForeignKey("Customer")]
        [Required]
        public string CustomerId { get; set; }
        [ForeignKey("List")]
        [MaxLength(10)]
        [Required]
        public string ListId { get; set; }
        public bool IsOwner { get; set; }
        public bool IsRemoved { get; set; }
        //public bool AddToList { get; set; }
        //public bool ShareList { get; set; }
        //public bool InviteCollaborators { get; set; }
        //public bool EditList { get; set; }
        //public bool DeleteList { get; set; }
        //public bool MoveItem { get; set; }
        //public bool RemoveItem { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual List List { get; set; }
        public virtual ICollection<ListProduct> ListProducts { get; set; }

        public ListCollaborator()
        {
            ListProducts = new HashSet<ListProduct>();
        }
    }
}