using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Keyword_In_KeywordGroup
    {
        [ForeignKey("KeywordGroup")]
        public int KeywordGroupId { get; set; }
        [ForeignKey("Keyword")]
        public int KeywordId { get; set; }

        public virtual KeywordGroup KeywordGroup { get; set; }
        public virtual Keyword Keyword { get; set; }
    }
}
