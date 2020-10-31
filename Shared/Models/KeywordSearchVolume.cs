using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccess.Models
{
    public class KeywordSearchVolume
    {
        [ForeignKey("Keyword")]
        [Required]
        public int KeywordId { get; set; }
        public DateTime Date { get; set; }

        public virtual Keyword Keyword { get; set; }
    }
}
