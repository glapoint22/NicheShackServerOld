using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class PageReferenceItem
    {
        public int Id { get; set; }

        [ForeignKey("Page")]
        public int PageId { get; set; }


        [ForeignKey("Niche")]
        public int? NicheId { get; set; }


        [ForeignKey("KeywordGroup")]
        public int? KeywordGroupId { get; set; }


        public virtual Page Page { get; set; }
        public virtual Niche Niche { get; set; }
        public virtual KeywordGroup KeywordGroup { get; set; }
    }
}
