using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Classes.Notifications
{
    public class NewNotification
    {
        public int? ProductId { get; set; }
        public string productName { get; set; }
        public string Thumbnail { get; set; }
        public int Type { get; set; }
        public string Email { get; set; }
        public int State { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }
    }
}
