using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Classes
{
    public struct TableOptions
    {
        public float Width { get; set; }
        public Background Background { get; set; }
        public string HorizontalAlignment { get; set; }
        public bool CreateRow { get; set; }
    }
}