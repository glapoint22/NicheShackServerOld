﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Website.Classes;
using Website.Models;
using static Website.Classes.Enums;

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
        public async Task<IEnumerable<ProductOrderDTO>> GetOrders(string customerId, string filter, string searchWords = "")
        {
            // This will return orders based on a time frame from the filter parameter or a single order based on an ordernumber from the searchwords parameter
            return await context.ProductOrders
                .AsNoTracking()
                .Where(new ProductOrderDTO(customerId, filter, searchWords))
                .Select(x => new ProductOrderDTO
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
                    ProductUrlTitle = x.Product.UrlTitle,
                    Hoplink = x.Product.Hoplink,
                    Products = x.OrderProducts
                        .Where(y => y.OrderId == x.Id)
                        .OrderByDescending(y => y.IsMain)
                        .Select(y => new OrderProductInfoDTO
                        {
                            Title = y.Title,
                            Type = ((OrderProductTypes)y.Type).ToString(),
                            Quantity = y.Type == 0 ? y.Quantity : 0,
                            Price = y.Price,
                            Image = y.IsMain ? y.ProductOrder.Product.Image : null
                        })
                })
                .ToListAsync();
        }








        // ....................................................................Get Order Products...........................................................................
        public async Task<IEnumerable<OrderProductQueryResultDTO>> GetOrderProducts(string customerId, string searchWords)
        {
            string[] searchWordsArray = searchWords.Split(' ').Select(x => "%" + x + "%").ToArray();

            // This will return products from orders based on customer Id and the searchWords parameter
            return await context.OrderProducts
                .AsNoTracking()
                .OrderByDescending(x => x.ProductOrder.Date)
                .ThenBy(x => x.OrderId)
                .Where(x => x.ProductOrder.CustomerId == customerId)
                .WhereAny(searchWordsArray.Select(w => (Expression<Func<OrderProduct, bool>>)(x => EF.Functions.Like(x.Title, w))).ToArray())
                .Select(x => new OrderProductQueryResultDTO
                {
                    Date = x.ProductOrder.Date.ToString("MMMM dd, yyyy"),
                    Title = x.Title,
                    Image = x.IsMain ? x.ProductOrder.Product.Image : null,
                    Hoplink = x.ProductOrder.Product.Hoplink,
                    OrderNumber = x.OrderId
                })
                .ToListAsync();
        }



        // ....................................................................Get Order Filters...........................................................................
        public async Task<List<KeyValuePair<string, string>>> GetOrderFilters(string customerId)
        {
            // Returns filter options that specify a time frame (ex. Last 30 days)
            List<KeyValuePair<string, string>> filterOptions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("last-30", "Last 30 days"),
                new KeyValuePair<string, string>("6-months", "Past 6 months"),
            };

            // Get years when products were bought from this customer
            List<KeyValuePair<string, string>> yearOptions = await context.ProductOrders
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .Select(x => new KeyValuePair<string, string>("year-" + x.Date.Year.ToString(), x.Date.Year.ToString()))
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
            }

            return title;
        }
    }
}
