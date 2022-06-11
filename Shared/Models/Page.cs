using DataAccess.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Page : IItem
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }


        
        [MaxLength(10)]
        public string UrlId { get; set; }


        
        [MaxLength(256)]
        public string UrlName { get; set; }


        
        public string Content { get; set; }

        public int PageType { get; set; }


        public virtual ICollection<PageReferenceItem> PageReferenceItems { get; set; }
        public virtual ICollection<PageKeyword> PageKeywords { get; set; }



        public Page()
        {
            PageReferenceItems = new HashSet<PageReferenceItem>();
            PageKeywords = new HashSet<PageKeyword>();
        }
    }
}