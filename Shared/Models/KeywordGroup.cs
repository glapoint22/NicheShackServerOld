using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class KeywordGroup
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Name { get; set; }

        public bool ForProduct { get; set; }


        public virtual ICollection<Keyword_In_KeywordGroup> Keywords_In_KeywordGroup { get; set; }
        public virtual ICollection<KeywordGroup_Belonging_To_Product> KeywordGroups_Belonging_To_Product { get; set; }

        public KeywordGroup ()
        {
            Keywords_In_KeywordGroup = new HashSet<Keyword_In_KeywordGroup>();
            KeywordGroups_Belonging_To_Product = new HashSet<KeywordGroup_Belonging_To_Product>();
        }
    }
}
