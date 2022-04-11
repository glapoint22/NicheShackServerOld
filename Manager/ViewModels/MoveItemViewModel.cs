using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.ViewModels
{
    public class MoveItemViewModel
    {
        public int ItemToBeMovedId { get; set; }
        public int DestinationItemId { get; set; }
    }
}
