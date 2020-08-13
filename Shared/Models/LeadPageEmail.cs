using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class LeadPageEmail
    {
        public int Id { get; set; }
        [ForeignKey("LeadPage")]
        public int LeadPageId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public virtual LeadPage LeadPage { get; set; }
    }
}