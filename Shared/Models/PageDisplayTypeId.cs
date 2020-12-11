using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccess.Models
{
    public class PageDisplayTypeId
    {
        public int Id { get; set; }

        [ForeignKey("Page")]
        public int PageId { get; set; }


        public int DisplayId { get; set; }


        public virtual Page Page { get; set; }
    }
}
