using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Services.Classes
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static implicit operator Item(HttpResponseMessage v)
        {
            throw new NotImplementedException();
        }
    }
}
