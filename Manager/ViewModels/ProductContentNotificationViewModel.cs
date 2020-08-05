using DataAccess.Interfaces;
using DataAccess.Models;
using Manager.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Manager.ViewModels
{
    public class ProductContentNotificationViewModel: GeneralNotificationViewModel, INotification, ISelect<Notification, ProductContentNotificationViewModel>
    {
        public IEnumerable<ProductContentViewModel> Content { get; set; }
        public IEnumerable<ProductPricePointViewModel> PricePoints { get; set; }



        public new IQueryable<ProductContentNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            return source.Select(x => new ProductContentNotificationViewModel
            {
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                ProductThumbnail = x.Product.Media.Url,
                VendorId = x.Product.VendorId,
                Hoplink = x.Product.Hoplink,


                CustomerText = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 0)
                .Select(z => new NotificationTextViewModel
                {
                    TimeStamp = z.TimeStamp,
                    Thumbnail = z.Customer.image,
                    Text = z.Text
                })
                .SingleOrDefault(),


                Content = x.Product.ProductContent.Select(c => new ProductContentViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = new ImageViewModel
                    {
                        Id = c.Media.Id,
                        Name = c.Media.Name,
                        Url = c.Media.Url
                    },
                    PriceIndices = c.Product.ProductPricePoints.Select(z => c.PriceIndices.Select(w => w.Index).Contains(z.Index))
                }),

                PricePoints = x.Product.ProductPricePoints.Select(p => new ProductPricePointViewModel
                {
                    Id = p.Id,
                    TextBefore = p.TextBefore,
                    WholeNumber = p.WholeNumber,
                    Decimal = p.Decimal,
                    TextAfter = p.TextAfter
                }),

                Notes = x.NotificationText
                .Where(z => z.NotificationId == x.Id && z.Type == 1)
                .Select(z => new NotificationTextViewModel
                {
                    TimeStamp = z.TimeStamp,
                    Thumbnail = z.Customer.image,
                    Text = z.Text
                })
                .SingleOrDefault()
            });
        }
    }
}
