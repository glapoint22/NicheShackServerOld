using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ListProduct
    {
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [ForeignKey("Collaborator")]
        public int CollaboratorId { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Product Product { get; set; }
        public virtual ListCollaborator Collaborator { get; set; }
    }
}