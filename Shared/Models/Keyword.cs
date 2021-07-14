using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Keyword: IItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }


        public virtual ICollection<ProductKeyword> ProductKeywords { get; set; }
        public virtual ICollection<KeywordSearchVolume> KeywordSearchVolumes { get; set; }
        public virtual ICollection<Keyword_In_KeywordGroup> Keywords_In_KeywordGroup { get; set; }

        public Keyword()
        {
            ProductKeywords = new HashSet<ProductKeyword>();
            KeywordSearchVolumes = new HashSet<KeywordSearchVolume>();
            Keywords_In_KeywordGroup = new HashSet<Keyword_In_KeywordGroup>();
        }
    }
}
