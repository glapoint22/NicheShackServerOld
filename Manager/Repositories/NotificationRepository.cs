using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes;
using Manager.Classes.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
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







        public async Task<IEnumerable> GetNotifications(bool isNew)
        {
            var allNotifications = await context.NotificationDetails.Where(x => isNew ? x.Notification.ArchiveDate == null : x.Notification.ArchiveDate != null)
                .Select(x => new
                {
                    ProductId = x.Notification.ProductId,
                    ProductName = x.Notification.Product.Name,
                    Thumbnail = x.Notification.Product.Media.Thumbnail,
                    Type = x.Notification.Type,
                    Email = x.Email != null ? x.Email : x.Customer.Email,
                    Date = x.TimeStamp,
                    ArchiveDate = x.Notification.ArchiveDate
                }).ToListAsync();



            var groupByTypeNotifications = allNotifications.GroupBy(x => x.Type, (key, n) => new
            {
                Type = key,
                Notifications = n.OrderByDescending(x => x.Date)
            }).ToList();



            var notifications = new List<NotificationItem>();

            foreach (var x in groupByTypeNotifications)
            {
                if (x.Type == 0)
                {
                    var messageNotifications = x.Notifications.GroupBy(y => new { y.Email, y.ArchiveDate }, (key, a) => new
                    {
                        Count = a.Count(),
                        Notification = a.FirstOrDefault()
                    }).Select(b => new NotificationItem
                    {
                        IsNew = isNew,
                        Type = b.Notification.Type,
                        ProductId = b.Notification.ProductId,
                        Thumbnail = b.Notification.Thumbnail,
                        Email = b.Notification.Email,
                        Date = b.Notification.Date,
                        Count = b.Count,
                        ArchiveDate = b.Notification.ArchiveDate
                    }).ToList();
                    notifications.AddRange(messageNotifications);
                }
                else
                {
                    var productNotifications = x.Notifications.GroupBy(y => new { y.ProductId, y.ArchiveDate }, (key, a) => new
                    {
                        Count = a.Count(),
                        Notification = a.FirstOrDefault()

                    }).Select(b => new NotificationItem
                    {
                        IsNew = isNew,
                        Type = b.Notification.Type,
                        ProductId = b.Notification.ProductId,
                        productName = b.Notification.ProductName,
                        Thumbnail = b.Notification.Thumbnail,
                        Email = b.Notification.Email,
                        Date = b.Notification.Date,
                        Count = b.Count,
                        ArchiveDate = b.Notification.ArchiveDate
                    }).ToList();

                    notifications.AddRange(productNotifications);
                }
            }

            return notifications.OrderByDescending(x => isNew ? x.Date : x.ArchiveDate);
        }





        public async Task<List<NotificationMessage>> GetMessageNotification(string email, int type, DateTime? archiveDate)
        {
            var notificationIds = await context.Notifications.Where(x => (x.NotificationDetails.Select(y => y.Customer).FirstOrDefault() == null ?
                                                                             (x.NotificationDetails.Select(y => y.Email).FirstOrDefault() == email) :
                                                                              x.NotificationDetails.Select(y => y.Customer.Email).FirstOrDefault() == email) &&
                                                                          x.Type == type &&
                                                                          x.ArchiveDate == archiveDate)
                                                             .Select(x => x.Id).ToListAsync();

            List<NotificationMessage> users = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new NotificationMessage
            {
                MessageId = x.NotificationId,
                Date = x.TimeStamp,
                SenderName = x.Name,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Email,
                Text = x.Text,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
                EmployeeReplyId = x.NotificationEmployeeNoteId,
                EmployeeFirstName = x.NotificationEmployeeNotes.Customer.FirstName,
                EmployeeLastName = x.NotificationEmployeeNotes.Customer.LastName,
                EmployeeImage = x.NotificationEmployeeNotes.Customer.Image,
                ReplyDate = x.NotificationEmployeeNotes.TimeStamp,
                Reply = x.NotificationEmployeeNotes.Text
            }).OrderByDescending(x => x.Date).ToListAsync();

            return users;
        }













        public async Task<NotificationReview> GetReviewNotification(int productId, int type, DateTime? archiveDate)
        {
            var notificationIds = await context.Notifications.Where(x => x.ProductId == productId &&
                                                                         x.Type == type &&
                                                                         x.ArchiveDate == archiveDate)
                                                            .Select(x => x.Id).ToListAsync();

            List<NotificationUser> users = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new NotificationUser
            {
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.Text,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
            }).ToListAsync();





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
                Text = x.Text
            }).FirstOrDefaultAsync();





            var notificationEmployeeNoteId = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId) && x.NotificationEmployeeNoteId != null).Select(x => x.NotificationEmployeeNoteId).FirstOrDefaultAsync();
            List<NotificationEmployee> employees = await context.NotificationEmployeeNotes.Where(x => x.Id == notificationEmployeeNoteId).Select(x => new NotificationEmployee
            {
                NoteId = x.Id,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.Text
            }).ToListAsync();






            NotificationReview notification = new NotificationReview
            {
                Users = users,
                ReviewWriter = reviewWriter,
                Employees = employees
            };

            return notification;
        }






        public async Task<NotificationProduct> GetProductNotification(int productId, int type, DateTime? archiveDate)
        {
            var notificationIds = await context.Notifications.Where(x => x.ProductId == productId &&
                                                                         x.Type == type &&
                                                                         x.ArchiveDate == archiveDate)
                                                            .Select(x => x.Id).ToListAsync();

            List<NotificationUser> users = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId)).Select(x => new NotificationUser
            {
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.Text,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending
            }).ToListAsync();


            var notificationEmployeeNoteId = await context.NotificationDetails.Where(x => notificationIds.Contains(x.NotificationId) && x.NotificationEmployeeNoteId != null).Select(x => x.NotificationEmployeeNoteId).FirstOrDefaultAsync();
            List<NotificationEmployee> employees = await context.NotificationEmployeeNotes.Where(x => x.Id == notificationEmployeeNoteId).Select(x => new NotificationEmployee
            {
                NoteId = notificationEmployeeNoteId,
                Date = x.TimeStamp,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.Text
            }).ToListAsync();


            NotificationProduct notification = new NotificationProduct
            {
                Users = users,
                Employees = employees
            };

            return notification;
        }

    }
}
