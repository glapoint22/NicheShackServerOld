using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class List
    {
        [MaxLength(32)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [MaxLength(32)]
        public string CollaborateId { get; set; }
        public virtual ICollection<ListCollaborator> Collaborators { get; set; }

        public List()
        {
            Collaborators = new HashSet<ListCollaborator>();
        }
    }
}
