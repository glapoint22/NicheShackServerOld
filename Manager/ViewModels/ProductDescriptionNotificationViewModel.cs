using DataAccess.Interfaces;
using DataAccess.Models;
using System.Linq;

namespace Manager.ViewModels
{
    //public class ProductDescriptionNotificationViewModel: GeneralNotificationViewModel, IQueryableSelect<Notification, ProductDescriptionNotificationViewModel>
    //{
    //    public string ProductDescription { get; set; }

    //    public new IQueryable<ProductDescriptionNotificationViewModel> Select(IQueryable<Notification> source)
    //    {
    //        GeneralNotificationViewModel generalNotificationViewModel = base.Select(source).SingleOrDefault();

    //        return source.Select(x => new ProductDescriptionNotificationViewModel
    //        {
    //            Name = generalNotificationViewModel.Name,
    //            CustomerText = generalNotificationViewModel.CustomerText,
    //            Notes = generalNotificationViewModel.Notes,
    //            ProductId = generalNotificationViewModel.ProductId,
    //            ProductName = generalNotificationViewModel.ProductName,
    //            ProductThumbnail = generalNotificationViewModel.ProductThumbnail,
    //            VendorId = generalNotificationViewModel.VendorId,
    //            Hoplink = generalNotificationViewModel.Hoplink,
    //            ProductDescription = x.Product.Description,
    //            Type = x.Type
    //        });
    //    }
    //}
}
