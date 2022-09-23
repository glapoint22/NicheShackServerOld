using DataAccess.Models;
using DataAccess.Repositories;
using Manager.Classes;
using Manager.Classes.Notifications;
using Microsoft.EntityFrameworkCore;
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







        public async Task<List<NotificationItem>> GetNotifications(bool isNew)
        {
            var allNotifications = await context.Notifications.Where(x => isNew ?

            // ---- NEW LIST ---- \\

            // Count all the notifications that DO NOT belong to an archived group
            x.NotificationGroup.ArchiveDate == null ||
            // but if it's a message notification that belongs to an archive group and
            // that message notification has NOT been archived, then count that one too
            (x.Type == (int)NotificationType.Message && !x.MessageArchived) :


            // ---- ARCHIVE LIST ---- \\

            // Count all the notifications that DO belong to an archived group
            x.NotificationGroup.ArchiveDate != null ||
            // but if it's a message notification that DOES NOT belong to an archive group and
            // that message notification HAS been archived, then count that one too
            (x.Type == (int)NotificationType.Message && x.MessageArchived))

                .Select(x => new
                {
                    Id = x.Id,
                    NotificationGroupId = x.NotificationGroupId,
                    Email = x.Type == (int)NotificationType.Message ?
                    x.NonAccountUserEmail != null ? x.NonAccountUserEmail :
                    x.Customer.Email :
                    null,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    ProductImage = x.Product.Media.Thumbnail,
                    NotificationType = x.Type,
                    CreationDate = x.CreationDate,
                    ArchiveDate = x.NotificationGroup.ArchiveDate,
                    Count = x.Type == (int)NotificationType.Message ? x.NotificationGroup.Notifications.Where(y => isNew ? !y.MessageArchived : y.MessageArchived).Count() : x.NotificationGroup.Notifications.Count()
                }).ToListAsync();


            List<NotificationItem> notifications = allNotifications
            // Group each notification by the group they belong to
            .GroupBy(x => x.NotificationGroupId, (key, n) => n
            // Then order each notification in each group by the most recent date
            .OrderByDescending(y => y.CreationDate)
            // And then return a list that consists of only the first notification of each group
            .FirstOrDefault())
            // Then take that list and order it by either the creation date (if we're compiling a New list) or the archive date (if we're compiling an Archive list)
            .OrderByDescending(x => isNew ? x.CreationDate : x.ArchiveDate)
            .Select(x => new NotificationItem
            {
                Id = x.Id,
                NotificationGroupId = x.NotificationGroupId,
                NotificationType = x.NotificationType,
                Email = x.Email,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Image = x.ProductImage,
                IsNew = isNew,
                CreationDate = x.CreationDate,
                Count = x.Count
            }).ToList();

            return notifications;
        }






        public async Task<List<NotificationMessage>> GetMessageNotification(int notificationGroupId, bool isNew)
        {
            List<NotificationMessage> message = await context.Notifications.Where(x => x.NotificationGroupId == notificationGroupId && (isNew ? !x.MessageArchived : x.MessageArchived)).Select(x => new NotificationMessage
            {
                NotificationId = x.Id,
                UserId = x.Customer.Id,
                UserName = x.NonAccountUserName,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.NonAccountUserEmail != null ? x.NonAccountUserEmail : x.Customer.Email,
                Text = x.UserComment,
                Date = x.CreationDate,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.NonAccountUserEmail != null ? context.BlockedNonAccountEmails.Where(y => y.Email == x.NonAccountUserEmail).FirstOrDefault() == null ? false : true : x.Customer.BlockNotificationSending,
                EmployeeMessageId = x.EmployeeMessageId,
                EmployeeFirstName = x.NotificationEmployeeMessage.Customer.FirstName,
                EmployeeLastName = x.NotificationEmployeeMessage.Customer.LastName,
                EmployeeImage = x.NotificationEmployeeMessage.Customer.Image,
                EmployeeMessageDate = x.NotificationEmployeeMessage.CreationDate,
                EmployeeMessage = x.NotificationEmployeeMessage.Message
            }).OrderByDescending(x => x.Date).ToListAsync();

            return message;
        }







        public async Task<NotificationReview> GetReviewNotification(int notificationGroupId)
        {
            var review = await context.Notifications.Where(x => x.NotificationGroupId == notificationGroupId).Select(x => new
            {
                Id = x.ProductReview.Id,
                Deleted = x.ProductReview.Deleted
            }).FirstOrDefaultAsync();

            List<NotificationUser> users = await context.Notifications.Where(x => x.NotificationGroupId == notificationGroupId).Select(x => new NotificationUser
            {
                UserId = x.Customer.Id,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.UserComment,
                Date = x.CreationDate,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
            }).ToListAsync();





            var reviewId = await context.Notifications.Where(x => x.NotificationGroupId == notificationGroupId).Select(x => x.ReviewId).FirstOrDefaultAsync();
            NotificationReviewWriter reviewWriter = await context.ProductReviews.Where(x => x.Id == reviewId).Select(x => new NotificationReviewWriter
            {
                UserId = x.Customer.Id,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.Text,
                Date = x.Date,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending,
                ReviewTitle = x.Title
            }).FirstOrDefaultAsync();





            List<NotificationProfile> employees = await context.NotificationEmployeeNotes.Where(x => x.NotificationGroupId == notificationGroupId).Select(x => new NotificationProfile
            {
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.Note,
                Date = x.CreationDate
            }).ToListAsync();




            NotificationReview notification = new NotificationReview
            {
                ReviewId = review.Id,
                ReviewDeleted = review.Deleted,
                Users = users,
                ReviewWriter = reviewWriter,
                Employees = employees
            };

            return notification;
        }





        public async Task<NotificationProduct> GetProductNotification(int notificationGroupId)
        {
            var product = await context.Notifications.Where(x => x.NotificationGroupId == notificationGroupId).Select(x => new
            {
                Hoplink = x.Product.Hoplink,
                Disabled = x.Product.Disabled
            }).FirstOrDefaultAsync();



            List<NotificationUser> users = await context.Notifications.Where(x => x.NotificationGroupId == notificationGroupId).Select(x => new NotificationUser
            {
                UserId = x.Customer.Id,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.UserComment,
                Date = x.CreationDate,
                NoncompliantStrikes = x.Customer.NoncompliantStrikes,
                BlockNotificationSending = x.Customer.BlockNotificationSending
            }).ToListAsync();


            List<NotificationProfile> employees = await context.NotificationEmployeeNotes.Where(x => x.NotificationGroupId == notificationGroupId).Select(x => new NotificationProfile
            {
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                Image = x.Customer.Image,
                Email = x.Customer.Email,
                Text = x.Note,
                Date = x.CreationDate
            }).ToListAsync();


            NotificationProduct notification = new NotificationProduct
            {
                ProductHoplink = product.Hoplink,
                ProductDisabled = product.Disabled,
                Users = users,
                Employees = employees
            };

            return notification;
        }
    }
}
