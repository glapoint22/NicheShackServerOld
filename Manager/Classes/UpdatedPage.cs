using DataAccess.ViewModels;
using System.Collections;
using System.Collections.Generic;
using static Manager.Classes.Utility;

namespace Manager.Classes
{
    public struct UpdatedPage
    {
        public int PageId { get; set; }
        public string Name { get; set; }
        public PageDisplayType DisplayType { get; set; }
        public IEnumerable<ItemViewModel> DisplayItems { get; set; }
        public string Content { get; set; }
    }
}