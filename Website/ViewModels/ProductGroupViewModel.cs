using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.ViewModels
{
    public class ProductGroupViewModel
    {
        public string Caption { get; set; }
        public List<QueriedProduct> Products { get; set; }
    }
}
