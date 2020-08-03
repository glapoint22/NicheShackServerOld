using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class LeadPage
    {
        public int Id { get; set; }
        [ForeignKey("Niche")]
        public int NicheId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public virtual Niche Niche { get; set; }
        public virtual ICollection<LeadPageEmail> LeadPageEmails { get; set; }

        public LeadPage()
        {
            LeadPageEmails = new HashSet<LeadPageEmail>();
        }
    }
}