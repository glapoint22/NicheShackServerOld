using DataAccess.Interfaces;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.ViewModels
{
    public class ReviewComplaintNotificationViewModel: GeneralNotificationViewModel, ISelect<Notification, ReviewComplaintNotificationViewModel>
    {
        public NotificationTextViewModel Review { get; set; }

        public new IQueryable<ReviewComplaintNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            GeneralNotificationViewModel generalNotificationViewModel = base.ViewModelSelect(source).SingleOrDefault();

            return source.Select(x => new ReviewComplaintNotificationViewModel
            {
                Name = generalNotificationViewModel.Name,
                CustomerText = generalNotificationViewModel.CustomerText,
                Notes = generalNotificationViewModel.Notes,
                ProductId = generalNotificationViewModel.ProductId,
                ProductName = generalNotificationViewModel.ProductName,
                ProductThumbnail = generalNotificationViewModel.ProductThumbnail,
                VendorId = generalNotificationViewModel.VendorId,
                Hoplink = generalNotificationViewModel.Hoplink,
                Type = x.Type,
                Review = x.NotificationText.Select(y => new NotificationTextViewModel
                {
                    TimeStamp = y.ProductReview.Date.ToString("MMMM dd, yyyy hh:mm tt"),
                    Thumbnail = y.ProductReview.Customer.Image,
                    Text = y.ProductReview.Text
                }).SingleOrDefault()
            });
        }
    }
}
