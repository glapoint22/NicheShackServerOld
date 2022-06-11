using Manager.ViewModels;
using Services.Classes;
using System.Collections.Generic;

namespace Manager.Classes
{
    public struct PageData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PageType { get; set; }
        public PageContent Content { get; set; }
        public IEnumerable<Item> PageReferenceItems { get; set; }
    }
}
