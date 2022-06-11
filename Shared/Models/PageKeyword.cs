using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    public class PageKeyword
    {
        public int Id { get; set; }

        [ForeignKey("Page")]
        [Required]
        public int PageId { get; set; }


        [ForeignKey("Keyword_In_KeywordGroup")]
        [Required]
        public int KeywordInKeywordGroupId { get; set; }



        public virtual Page Page { get; set; }
        public virtual Keyword_In_KeywordGroup KeywordInKeywordGroup { get; set; }
    }
}