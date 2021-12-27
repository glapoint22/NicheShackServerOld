using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.Classes;
using static Website.Classes.Enums;
using Website.ViewModels;

namespace Website.Repositories
{
    public class ProductOrderRepository : Repository<ProductOrder>, IProductOrderRepository
    {
        private readonly NicheShackContext context;

        public ProductOrderRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }



        // ..................................................................................Get Orders.....................................................................
        public async Task<IEnumerable<ProductOrderViewModel>> GetOrders(string customerId, string filter, string searchWords = "")
        {
            // This will return orders based on a time frame from the filter parameter or a single order based on an ordernumber from the searchwords parameter
            return await context.ProductOrders
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .Where(new ProductOrderViewModel(customerId, filter, searchWords))
                .Select(x => new ProductOrderViewModel
                {
                    OrderNumber = x.Id,
                    Date = x.Date.ToString("MMMM dd, yyyy"),
                    PaymentMethod = GetPaymentMethod(x.PaymentMethod),
                    PaymentMethodImg = GetPaymentMethodImg(x.PaymentMethod),
                    Subtotal = x.Subtotal,
                    ShippingHandling = x.ShippingHandling,
                    Discount = x.Discount,
                    Tax = x.Tax,
                    Total = x.Total,
                    Hoplink = x.Product.Hoplink + (x.Product.Hoplink.Contains('?') ? "&" : "?") + "tid=" + x.Product.UrlId + "_" + customerId,
                    ProductId = x.ProductId,
                    Products = x.OrderProducts
                        .Where(y => y.OrderId == x.Id)
                        .OrderByDescending(y => y.LineItemType == "ORIGINAL")
                        .Select(y => new OrderProductInfoViewModel
                        {
                            Name = y.Name,
                            Quantity = y.Quantity > 1 ? y.Quantity : 0,
                            Price = y.Price,
                            Image = y.LineItemType == "ORIGINAL" ? new ImageViewModel
                            {
                                Name = y.ProductOrder.Product.Media.Name,
                                Url = y.ProductOrder.Product.Media.Image
                            } : null,
                            RebillFrequency = y.RebillFrequency,
                            RebillAmount = y.RebillAmount,
                            PaymentsRemaining = y.PaymentsRemaining
                        })
                })
                .ToListAsync();
        }








        // ....................................................................Get Order Products...........................................................................
        public async Task<IEnumerable<OrderProductQueryResultViewModel>> GetOrderProducts(string customerId, string searchWords)
        {
            string[] searchWordsArray = searchWords.Split(' ').Select(x => "%" + x + "%").ToArray();

            // This will return products from orders based on customer Id and the searchWords parameter
            return await context.OrderProducts
                .AsNoTracking()
                .OrderByDescending(x => x.ProductOrder.Date)
                .ThenBy(x => x.OrderId)
                .Where(x => x.ProductOrder.CustomerId == customerId)
                .WhereAny(searchWordsArray.Select(w => (Expression<Func<OrderProduct, bool>>)(x => EF.Functions.Like(x.Name, w))).ToArray())
                .Select(x => new OrderProductQueryResultViewModel
                {
                    Date = x.ProductOrder.Date.ToString("MMMM dd, yyyy"),
                    Name = x.Name,
                    Image = x.LineItemType == "ORIGINAL" ? new ImageViewModel
                    {
                        Name = x.ProductOrder.Product.Media.Name,
                        Url = x.ProductOrder.Product.Media.Image
                    } : new ImageViewModel { },
                    Hoplink = x.ProductOrder.Product.Hoplink + (x.ProductOrder.Product.Hoplink.Contains('?') ? "&" : "?") + "tid=" + x.ProductOrder.Product.UrlId + "_" + customerId,
                    OrderNumber = x.OrderId,
                    ProductUrlId = x.ProductOrder.Product.UrlId,
                    UrlName = x.ProductOrder.Product.UrlName
                })
                .ToListAsync();
        }



        // ....................................................................Get Order Filters...........................................................................
        public async Task<List<KeyValuePair<string, string>>> GetOrderFilters(string customerId)
        {
            // Returns filter options that specify a time frame (ex. Last 30 days)
            List<KeyValuePair<string, string>> filterOptions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Last 30 days", "last-30"),
                new KeyValuePair<string, string>("Past 6 months", "6-months"),
            };

            // Get years when products were bought from this customer
            List<KeyValuePair<string, string>> yearOptions = await context.ProductOrders
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .Select(x => new KeyValuePair<string, string>(x.Date.Year.ToString(), "year-" + x.Date.Year.ToString()))
                .Distinct()
                .ToListAsync();

            // Combine the two filters together and return
            filterOptions.AddRange(yearOptions.OrderByDescending(x => x.Key));
            return filterOptions;
        }













        // .............................................................................Get Payment Method Img..............................................................
        private static string GetPaymentMethodImg(int paymentMethodIndex)
        {
            string img = string.Empty;

            switch (paymentMethodIndex)
            {
                case 0:
                    img = "paypal.png";
                    break;
                case 1:
                    img = "visa.png";
                    break;
                case 2:
                    img = "master_card.png";
                    break;
                case 3:
                    img = "discover.png";
                    break;
                case 4:
                    img = "amex.png";
                    break;
                case 5:
                    img = "solo.png";
                    break;
                case 6:
                    img = "diners_club.png";
                    break;
                case 7:
                    img = "maestro.png";
                    break;
                case 8:
                    img = "master_card.png";
                    break;
            }

            return img;
        }





        // .............................................................................Get Payment Method..................................................................
        private static string GetPaymentMethod(int paymentMethodIndex)
        {
            string title = string.Empty;

            switch (paymentMethodIndex)
            {
                case 0:
                    title = "Paypal";
                    break;
                case 1:
                    title = "Visa";
                    break;
                case 2:
                    title = "Mastercard";
                    break;
                case 3:
                    title = "Discover";
                    break;
                case 4:
                    title = "American Express";
                    break;
                case 5:
                    title = "Solo";
                    break;
                case 6:
                    title = "Diners Club";
                    break;
                case 7:
                    title = "Maestro";
                    break;
                case 8:
                    title = "Mastercard";
                    break;
            }

            return title;
        }
    }
}
