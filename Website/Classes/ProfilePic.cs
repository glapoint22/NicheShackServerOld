using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Classes
{
    public class ProfilePic
    {
        public string Image { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int CropLeft { get; set; }
        public int CropTop { get; set; }
    }
}
