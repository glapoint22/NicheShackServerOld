using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Manager.Classes.Utility;

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

            var messageNotifications = await context.Notifications.Where(x => x.State == 0 && x.Type == (int)NotificationType.Message && x.NotificationDetails.Select(z => z.Email).FirstOrDefault() != null).Select(x => new
            {
                ProductId = x.ProductId,
                Thumbnail = x.Product.Media.Thumbnail,
                Type = (int)NotificationType.Message,
                Email = x.NotificationDetails.Select(y => y.Email).FirstOrDefault(),
                State = x.State,
                count = context.NotificationDetails.Where(y => y.Notification.State == x.State && y.Notification.Type == x.Type && y.Email == x.NotificationDetails.Select(z => z.Email).FirstOrDefault()).Count()
            }).ToListAsync();


            var notifications = await context.Notifications.Where(x => x.State == 0 && x.Type != (int)NotificationType.Message).Select(x => new
            {
                ProductId = x.ProductId,
                Thumbnail = x.Product.Media.Thumbnail,
                Type = x.Type,
                Email = x.NotificationDetails.Select(y => y.Email).FirstOrDefault(),
                State = x.State,
                count = context.Notifications.Where(y => y.State == x.State && y.ProductId == x.ProductId && y.Type == x.Type).Count()
            }).ToListAsync();


            var newNotifications = notifications.Concat(messageNotifications).Distinct();

            return newNotifications;
        }






        public async Task<IEnumerable> GetMessageNotification(int type, int state, string email)
        {
            var notificationIds = await context.Notifications.Where(x => x.Type == type && x.State == state && x.NotificationDetails.Select(y => y.Email).FirstOrDefault() == email).Select(x => x.Id).ToListAsync();


            var user = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new
            {
                NotificationId = x.NotificationId,
                Date = x.TimeStamp,
                Email = x.Email,
                Name = x.Name,
                Message = x.Text
            }).ToListAsync();


            var employee = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new
            {
                NotificationId = x.NotificationId,
                NotificationDetailsId = x.Id,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Email = x.Customer.Email,
                Image = x.Customer.Image,
                Reply = x.Text
            }).ToListAsync();


            var notification = user.GroupJoin(employee, x => x.NotificationId, x => x.NotificationId, (user, employee) => new
            {
                user,
                employee

            }).SelectMany(z => z.employee.DefaultIfEmpty(), (u, e) => new
            {

                NotificationId = u.user.NotificationId,

                User = new
                {
                    Date = u.user.Date,
                    Email = u.user.Email,
                    Name = u.user.Name,
                    Message = u.user.Message
                },

                Employee = new
                {
                    NotificationDetailsId = e == null ? 0 : e.NotificationDetailsId,
                    Date = e == null ? new DateTime() : e.Date,
                    FirstName = e == null ? "" : e.FirstName,
                    LastName = e == null ? "" : e.LastName,
                    Email = e == null ? "" : e.Email,
                    Image = e == null ? "" : e.Image,
                    Reply = e == null ? "" : e.Reply
                }
            });

            return notification;
        }













        public async Task<NotificationReviewComplaint> GetReviewComplaintNotification(int productId, int type, int state)
        {
            var notificationIds = await context.Notifications.Where(x => x.ProductId == productId && x.Type == type && x.State == state).Select(x => x.Id).ToListAsync();


            IEnumerable<NotificationReviewUser> user = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new NotificationReviewUser
            {
                Date = x.TimeStamp,
                Email = x.Customer.Email,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                //NoncompliantStrikes = x.Customer.NoncompliantStrikes;
                //BlockNotificationSending = x.Customer.BlockNotificationSending;
                NotificationId = x.NotificationId,
                Complaint = x.Text
            }).ToListAsync();




            //var employee = await context.NotificationText.Where(x => notificationIds.Contains(x.NotificationId) && x.Type == (int)NotificationDetailsType.Employee).Select(x => new
            //{
            //    NotificationId = x.NotificationId,
            //    NotificationDetailsId = x.Id,
            //    Date = x.TimeStamp,
            //    FirstName = x.Customer.FirstName,
            //    LastName = x.Customer.LastName,
            //    Email = x.Customer.Email,
            //    Image = x.Customer.Image,
            //    Note = x.Text
            //}).ToListAsync();


            var reviewId = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => x.ReviewId).FirstOrDefaultAsync();

            NotificationReviewWriter reviewWriter = await context.ProductReviews.Where(x => x.Id == reviewId)
                .Select(x => new NotificationReviewWriter
                {
                    Date = x.Date,
                    Email = x.Customer.Email,
                    FirstName = x.Customer.FirstName,
                    LastName = x.Customer.LastName,
                    Image = x.Customer.Image,
                    //NoncompliantStrikes = x.Customer.NoncompliantStrikes;
                    //BlockNotificationSending = x.Customer.BlockNotificationSending;
                    ReviewTitle = x.Title,
                    Review = x.Text
                }).FirstOrDefaultAsync();





            NotificationReviewComplaint notification = new NotificationReviewComplaint
            {
                User = user,
                //Employee = employee,
                ReviewWriter = reviewWriter
            };











            return notification;
        }




















        public async Task<IEnumerable> GetProductNotification(int productId, int type, int state)
        {
            var notificationIds = await context.Notifications.Where(x => x.ProductId == productId && x.Type == type && x.State == state).Select(x => x.Id).ToListAsync();




            var user = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new
            {
                NotificationId = x.NotificationId,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Email = x.Customer.Email,
                Image = x.Customer.Image,
                //NoncompliantStrikes = x.Customer.NoncompliantStrikes;
                //BlockNotificationSending = x.Customer.BlockNotificationSending;
                Comment = x.Text
            }).ToListAsync();


            var employee = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new
            {
                NotificationId = x.NotificationId,
                NotificationDetailsId = x.Id,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Email = x.Customer.Email,
                Image = x.Customer.Image,
                Note = x.Text
            }).ToListAsync();



            var notification = user.GroupJoin(employee, x => x.NotificationId, x => x.NotificationId, (user, employee) => new
            {
                user,
                employee

            }).SelectMany(z => z.employee.DefaultIfEmpty(), (u, e) => new
            {

                NotificationId = u.user.NotificationId,

                User = new
                {
                    Date = u.user.Date,
                    FirstName = u.user.FirstName,
                    LastName = u.user.LastName,
                    Email = u.user.Email,
                    Image = u.user.Image,
                    //NoncompliantStrikes = u.user.NoncompliantStrikes;
                    //BlockNotificationSending = u.user.BlockNotificationSending;
                    Comment = u.user.Comment
                },

                Employee = new
                {
                    NotificationDetailsId = e == null ? 0 : e.NotificationDetailsId,
                    Date = e == null ? new DateTime() : e.Date,
                    FirstName = e == null ? "" : e.FirstName,
                    LastName = e == null ? "" : e.LastName,
                    Email = e == null ? "" : e.Email,
                    Image = e == null ? "" : e.Image,
                    Note = e == null ? "" : e.Note
                }
            });

            return notification;
        }





    }
}
