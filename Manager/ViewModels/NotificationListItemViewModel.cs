using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Classes;
using System.Collections.Generic;
using System.Linq;

namespace Manager.ViewModels
{
    public class NotificationListItemViewModel: ISelect<Notification, NotificationListItemViewModel>
    {
        public int? ProductId { get; set; }
        public string Name { get; set; }
        public string ListIcon { get; set; }
        public int Type { get; set; }
        public int State { get; set; }



        public IQueryable<NotificationListItemViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            return source.Select(x => new NotificationListItemViewModel
            {
                State = x.State,
                ProductId = x.ProductId,
                Type = x.Type,
                Name = Utility.GetNotificationName(x.Type),
                ListIcon = x.Type > 1 ? x.Product.Media.Url : x.Type == 0 ? "message.png" : "review-complaint.png"
            }).Distinct();
        }
    }
}
