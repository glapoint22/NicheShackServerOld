using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DataAccess.Models
{
    public class Subproduct
    {
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [ForeignKey("Media")]
        public int? ImageId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        
        public double Value { get; set; }
        public int Type { get; set; }


        public virtual Media Media { get; set; }
        public virtual Product Product { get; set; }
    }
}
