using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.ViewModels;

namespace Website.Classes
{
    public class EmailParams
    {
        public Person Collaborator { get; set; }
        public ListViewModel List { get; set; }
        public ProductData Product { get; set; }
        public IEnumerable<Recipient> Recipients { get; set; }
        public string Host { get; set; }
    }
}
