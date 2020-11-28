﻿using DataAccess.Interfaces;
using DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace Manager.ViewModels
{
    public class ProductContentNotificationViewModel: GeneralNotificationViewModel, IQueryableSelect<Notification, ProductContentNotificationViewModel>
    {
        public IEnumerable<ProductContentViewModel> Content { get; set; }
        public IEnumerable<ProductPricePointViewModel> PricePoints { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }


        public new IQueryable<ProductContentNotificationViewModel> Select(IQueryable<Notification> source)
        {
            GeneralNotificationViewModel generalNotificationViewModel = base.Select(source).SingleOrDefault();

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
                MinPrice = x.Product.MinPrice,
                MaxPrice = x.Product.MaxPrice,
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
                }),
                Type = x.Type
            });
        }
    }
}
