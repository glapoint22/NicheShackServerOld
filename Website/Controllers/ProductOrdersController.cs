using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Website.Classes;
using Website.Repositories;
using static Website.Classes.Enums;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductOrdersController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;

        public ProductOrdersController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
        }



        // ..................................................................................Get Orders.....................................................................
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetOrders(string filter = "last-30", string search = "")
        {
            // Get the customer id from the claims
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // If there are search words
            if (search != string.Empty)
            {
                // This will search for orders with an order id
                var orders = await unitOfWork.ProductOrders.GetOrders(customerId, filter, search);
                if (orders.Count() > 0)
                {
                    // Return an order with the given order id
                    return Ok(new
                    {
                        orders,
                        displayType = "order"
                    });
                }
                else
                {
                    // Search for products in orders
                    return Ok(new
                    {
                        products = await unitOfWork.ProductOrders.GetOrderProducts(customerId, search),
                        displayType = "product"
                    });
                }
            }




            // Return orders based on a time frame
            return Ok(new
            {
                orders = await unitOfWork.ProductOrders.GetOrders(customerId, filter),
                filters = await unitOfWork.ProductOrders.GetOrderFilters(customerId),
                displayType = "order"
            });
        }





        private string DecryptNotification(EncryptedOrderNotification notification)
        {
            string decryptedString = null;

            byte[] inputBytes = Encoding.UTF8.GetBytes(configuration["OrderNotification:Key"]);

            SHA1 sha1 = SHA1.Create();
            byte[] key = sha1.ComputeHash(inputBytes);

            StringBuilder hex = new StringBuilder(key.Length * 2);
            foreach (byte b in key)
                hex.AppendFormat("{0:x2}", b);

            string secondPhaseKey = hex.ToString().Substring(0, 32);

            ASCIIEncoding asciiEncoding = new ASCIIEncoding();

            byte[] keyBytes = asciiEncoding.GetBytes(secondPhaseKey);
            byte[] iv = Convert.FromBase64String(notification.iv);


            using (RijndaelManaged rijndaelManaged = new RijndaelManaged
            {
                Key = keyBytes,
                IV = iv,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            })
            using (Stream memoryStream = new MemoryStream(Convert.FromBase64String(notification.notification)))
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(keyBytes, iv), CryptoStreamMode.Read))
            {
                decryptedString = new StreamReader(cryptoStream).ReadToEnd();
            }

            return decryptedString;
        }


        [HttpPost]
        [Route("PostOrder")]
        public async Task PostOrder(EncryptedOrderNotification encryptedOrderNotification)
        {
            OrderNotification orderNotification;

            try
            {
                string decryptedNotification = DecryptNotification(encryptedOrderNotification);

                orderNotification = JsonSerializer.Deserialize<OrderNotification>(decryptedNotification, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception e)
            {
                // Log the exception
                return;
            }



            if (orderNotification.TransactionType == null) return;


            if (orderNotification.TransactionType == "SALE" || orderNotification.TransactionType == "TEST_SALE")
            {

                // Id
                if (orderNotification.Receipt == null) return;
                string id = orderNotification.Receipt;


                // Do we have tracking codes?
                if (orderNotification.TrackingCodes == null || orderNotification.TrackingCodes.Count() == 0) return;

                // Split the tracking codes into product id && customer id
                string[] trackingCodes = orderNotification.TrackingCodes.ToArray()[0].Split('_');

                // Get the product id
                int productId = await unitOfWork.Products.Get(x => x.UrlId == trackingCodes[0], x => x.Id);
                if (productId == 0) return;


                // Get the customer id
                if (!await unitOfWork.Customers.Any(x => x.Id == trackingCodes[1])) return;
                string customerId = trackingCodes[1];


                // Payment method
                if (orderNotification.PaymentMethod == null) return;
                string paymentMethod = orderNotification.PaymentMethod;



                if (orderNotification.LineItems == null || orderNotification.LineItems.Count() == 0) return;

                double subtotal = 0;
                double tax = 0;
                double discount = 0;
                double shipping = 0;

                foreach (LineItem lineItem in orderNotification.LineItems)
                {
                    subtotal += lineItem.ProductPrice;
                    tax += lineItem.TaxAmount;
                    discount += lineItem.ProductDiscount;
                    shipping += lineItem.ShippingAmount;

                    OrderProduct orderProduct = new OrderProduct
                    {
                        OrderId = id,
                        Name = lineItem.ProductTitle,
                        Quantity = lineItem.Quantity,
                        Price = lineItem.ProductPrice,
                        LineItemType = lineItem.LineItemType,
                        RebillFrequency = lineItem.Recurring ? lineItem.PaymentPlan.RebillFrequency : null,
                        RebillAmount = lineItem.Recurring ? lineItem.PaymentPlan.RebillAmount : 0,
                        PaymentsRemaining = lineItem.Recurring ? lineItem.PaymentPlan.PaymentsRemaining : 0
                    };

                    unitOfWork.OrderProducts.Add(orderProduct);

                }

                ProductOrder productOrder = new ProductOrder
                {
                    Id = id,
                    ProductId = productId,
                    CustomerId = customerId,
                    Date = DateTime.Now,
                    PaymentMethod = (int)Enum.Parse(typeof(PaymentMethod), paymentMethod),
                    Subtotal = subtotal,
                    ShippingHandling = shipping,
                    Discount = discount,
                    Tax = tax,
                    Total = orderNotification.TotalOrderAmount
                };


                unitOfWork.ProductOrders.Add(productOrder);



                await unitOfWork.Save();
            }
        }
    }
}