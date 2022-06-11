using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class Keyword_In_KeywordGroup
    {
        public int Id { get; set; }

        [ForeignKey("KeywordGroup")]
        public int KeywordGroupId { get; set; }
        [ForeignKey("Keyword")]
        public int KeywordId { get; set; }

        public virtual KeywordGroup KeywordGroup { get; set; }
        public virtual Keyword Keyword { get; set; }
        public virtual ICollection<PageKeyword> PageKeywords { get; set; }



        public Keyword_In_KeywordGroup()
        {
            PageKeywords = new HashSet<PageKeyword>();
        }
    }
}
