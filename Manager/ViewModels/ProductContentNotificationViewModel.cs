using DataAccess.Interfaces;
using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Manager.ViewModels
{
    public class ProductContentNotificationViewModel: GeneralNotificationViewModel, ISelect<Notification, ProductContentNotificationViewModel>
    {
        public IEnumerable<ProductContentViewModel> Content { get; set; }
        public IEnumerable<ProductPricePointViewModel> PricePoints { get; set; }


        public new IQueryable<ProductContentNotificationViewModel> ViewModelSelect(IQueryable<Notification> source)
        {
            GeneralNotificationViewModel generalNotificationViewModel = base.ViewModelSelect(source).SingleOrDefault();

            return source.Select(x => new ProductContentNotificationViewModel
            {
                Name = generalNotificationViewModel.Name,
                CustomerText = generalNotificationViewModel.CustomerText,
                Notes = generalNotificationViewModel.Notes,
                ProductId = generalNotificationViewModel.ProductId,
                ProductName = generalNotificationViewModel.ProductName,
                ProductThumbnail = generalNotificationViewModel.ProductThumbnail,
                VendorId = generalNotificationViewModel.VendorId,
                Hoplink = generalNotificationViewModel.Hoplink,
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
                    PriceIndices = c.Product.ProductPricePoints.OrderBy(y => y.Index).Select(z => c.PriceIndices.Select(w => w.Index).Contains(z.Index))
                }),
                PricePoints = x.Product.ProductPricePoints.OrderBy(y => y.Index).Select(p => new ProductPricePointViewModel
                {
                    Id = p.Id,
                    TextBefore = p.TextBefore,
                    WholeNumber = p.WholeNumber,
                    Decimal = p.Decimal,
                    TextAfter = p.TextAfter
                })
            });
        }
    }
}
