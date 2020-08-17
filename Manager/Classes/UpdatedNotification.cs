using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes
{
    public struct UpdatedNotification
    {
        public int ProductId { get; set; }
        public int Type { get; set; }
        public int CurrentState { get; set; }
        public int DestinationState { get; set; }
    }
}
