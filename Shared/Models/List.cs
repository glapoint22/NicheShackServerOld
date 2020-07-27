using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class List
    {
        [MaxLength(10)]
        public string Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public string Description { get; set; }
        [MaxLength(10)]
        [Required]
        public string CollaborateId { get; set; }
        public virtual ICollection<ListCollaborator> Collaborators { get; set; }

        public List()
        {
            Collaborators = new HashSet<ListCollaborator>();
        }
    }
}
