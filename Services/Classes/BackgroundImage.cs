using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public class BackgroundImage: Image
    {
        public string Position { get; set; }
        public string Repeat { get; set; }
        public string Attachment { get; set; }
    }
}
