using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes
{
    public struct NewImage
    {
        public IFormFile Image { get; set; }
        public int Type { get; set; }
    }
}
