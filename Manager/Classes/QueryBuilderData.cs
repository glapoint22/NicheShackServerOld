using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes
{
    public struct QueryBuilderData
    {
        public IEnumerable<int> CategoryIds { get; set; }
        public IEnumerable<int> NicheIds { get; set; }
        public IEnumerable<string> Keywords { get; set; }
        public IEnumerable<int> ProductIds { get; set; }


        public IEnumerable<int> Where { get; set; }

    }
}