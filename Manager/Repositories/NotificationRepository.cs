﻿using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes;
using Manager.Classes.Notifications;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Repositories
{







    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly NicheShackContext context;

        public NotificationRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }







        public async Task<IEnumerable> GetNewNotifications()
        {
            var allNewNotifications = await context.NotificationDetails.Where(x => x.Notification.State == 0)
                .Select(x => new
                {
                    ProductId = x.Notification.ProductId,
                    ProductName = x.Notification.Product.Name,
                    Thumbnail = x.Notification.Product.Media.Thumbnail,
                    Type = x.Notification.Type,
                    Email = x.Email != null ? x.Email : x.Customer.Email,
                    Date = x.TimeStamp
                }).ToListAsync();



            var groupByTypeNotifications = allNewNotifications.GroupBy(x => x.Type, (key, n) => new
            {
                Type = key,
                Notifications = n.OrderByDescending(x => x.Date).ToList()
            }).ToList();



            var newNotifications = new List<NewNotification>();

            foreach (var x in groupByTypeNotifications)
            {

                if (x.Type == 0)
                {
                    var messageNotifications = x.Notifications.GroupBy(y => y.Email, (key, a) => new
                    {
                        Count = a.Count(),
                        Notification = a.FirstOrDefault()
                    }).Select(b => new NewNotification
                    {
                        State = 0,
                        Type = b.Notification.Type,
                        ProductId = b.Notification.ProductId,
                        Thumbnail = b.Notification.Thumbnail,
                        Email = b.Notification.Email,
                        Date = b.Notification.Date,
                        Count = b.Count
                    }).ToList();
                    newNotifications.AddRange(messageNotifications);
                }
                else
                {


                    var productNotifications = x.Notifications.GroupBy(y => y.ProductId, (key, a) => new
                    {
                        Count = a.Count(),
                        Notification = a.FirstOrDefault()

                    }).Select(b => new NewNotification
                    {
                        State = 0,
                        Type = b.Notification.Type,
                        ProductId = b.Notification.ProductId,
                        productName = b.Notification.ProductName,
                        Thumbnail = b.Notification.Thumbnail,
                        Email = b.Notification.Email,
                        Date = b.Notification.Date,
                        Count = b.Count
                    }).ToList();

                    newNotifications.AddRange(productNotifications);
                }
            }

            return newNotifications.OrderByDescending(x => x.Date);
        }












        public async Task<NotificationMessage> GetMessageNotification(int type, int state, string email)
        {
            var notificationIds = await context.Notifications.Where(x => x.Type == type && x.State == state &&
            (x.NotificationDetails.Select(z => z.Customer).FirstOrDefault() == null ?
                (x.NotificationDetails.Select(y => y.Email).FirstOrDefault() == email) : x.NotificationDetails.Select(z => z.Customer.Email).FirstOrDefault() == email)).Select(x => x.Id).ToListAsync();


            List<NotificationMessageUser> user = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new NotificationMessageUser
            {
                Email = x.Email,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
                NotificationId = x.NotificationId,
                Name = x.Name,
                Message = x.Text
            }).ToListAsync();



            List<NotificationMessageEmployee> employee = await context.NotificationEmployees.Where(x => notificationIds.Contains(x.Id)).Select(x => new NotificationMessageEmployee
            {
                Email = x.Customer.Email,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Reply = x.Note
            }).ToListAsync();


            NotificationMessage notification = new NotificationMessage
            {
                Users = user,
                Employee = employee
            };


            return notification;
        }













        public async Task<NotificationReviewComplaint> GetReviewComplaintNotification(int productId, int type, int state)
        {
            var notificationIds = await context.Notifications.Where(x => x.ProductId == productId && x.Type == type && x.State == state).Select(x => x.Id).ToListAsync();


            IEnumerable<NotificationReviewUser> user = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new NotificationReviewUser
            {
                Email = x.Customer.Email,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
                NotificationId = x.NotificationId,
                Complaint = x.Text
            }).ToListAsync();




            var employeeId = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => x.NotificationEmployeeId).FirstOrDefaultAsync();
            NotificationEmployeeDetails employee = await context.NotificationEmployees.Where(x => x.Id == employeeId).Select(x => new NotificationEmployeeDetails
            {
                Email = x.Customer.Email,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Note = x.Note
            }).FirstOrDefaultAsync();





            var reviewId = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => x.ReviewId).FirstOrDefaultAsync();
            NotificationReviewWriter reviewWriter = await context.ProductReviews.Where(x => x.Id == reviewId).Select(x => new NotificationReviewWriter
            {
                Email = x.Customer.Email,
                Date = x.Date,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
                ReviewTitle = x.Title,
                Review = x.Text
            }).FirstOrDefaultAsync();



            NotificationReviewComplaint notification = new NotificationReviewComplaint
            {
                Users = user,
                Employee = employee,
                ReviewWriter = reviewWriter
            };

            return notification;
        }




















        public async Task<NotificationProduct> GetProductNotification(int productId, int type, int state)
        {
            var notificationIds = await context.Notifications.Where(x => x.ProductId == productId && x.Type == type && x.State == state).Select(x => x.Id).ToListAsync();

            List<NotificationProductUser> user = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new NotificationProductUser
            {
                Email = x.Customer.Email,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
                NotificationId = x.NotificationId,
                Comment = x.Text
            }).ToListAsync();


            var employeeId = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => x.NotificationEmployeeId).FirstOrDefaultAsync();
            NotificationEmployeeDetails employee = await context.NotificationEmployees.Where(x => x.Id == employeeId).Select(x => new NotificationEmployeeDetails
            {
                Email = x.Customer.Email,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Note = x.Note
            }).FirstOrDefaultAsync();



            NotificationProduct notification = new NotificationProduct
            {
                Users = user,
                Employee = employee
            };

            return notification;
        }
    }
}
