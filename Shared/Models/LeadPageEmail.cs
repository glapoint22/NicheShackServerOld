using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class LeadPageEmail
    {
        public int Id { get; set; }
        [ForeignKey("LeadPage")]
        public int LeadPageId { get; set; }
        public string Content { get; set; }
        public string Subject { get; set; }
        public virtual LeadPage LeadPage { get; set; }
    }
}