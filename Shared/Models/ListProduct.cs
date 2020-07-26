using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class ListProduct
    {
        [ForeignKey("Product")]
        [MaxLength(10)]
        public string ProductId { get; set; }
        [ForeignKey("Collaborator")]
        public Guid CollaboratorId { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual Product Product { get; set; }
        public virtual ListCollaborator Collaborator { get; set; }
    }
}