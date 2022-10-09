using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Website.Classes.Notifications;
using static Website.Classes.Enums;




namespace Website.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly NicheShackContext context;

        public NotificationRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }





        public async Task CreateNotification(NewNotification newNotification, string userId)
        {
            // If a message is being sent from a non-account user
            if (newNotification.NonAccountEmail != null)
            {
                // Check to see if the email of that non-account user is on the block list
                var blockedNonAccountEmail = await context.BlockedNonAccountEmails.Where(x => x.Email == newNotification.NonAccountEmail).Select(x => x.Email).FirstOrDefaultAsync();
                // If so, don't create the notification
                if (blockedNonAccountEmail != null) return;

            // If it's anything other than a message from a non-account user
            }
            else
            {
                // Check to see if the user is on the block list
                var blockedUser = await context.Customers.Where(x => x.Id == userId).Select(x => x.BlockNotificationSending).FirstOrDefaultAsync();
                // If so, don't create the notification
                if (blockedUser) return;
            }


            // First, check to see if a notification group for the type of notification that we're going to create already exists
            NotificationGroup notificationGroup = await context.Notifications.Where(x =>

            // If we're creating UserName or UserImage notification
            ((newNotification.Type == (int)NotificationType.UserName || newNotification.Type == (int)NotificationType.UserImage) && x.Type == newNotification.Type && x.UserId == userId) ||

            // If we're creating a Message notification
            (newNotification.Type == (int)NotificationType.Message &&
                    // If it's a message from a user with an account
                    (newNotification.NonAccountEmail == null && x.Type == newNotification.Type && x.Customer.Email == newNotification.Email) ||
                    // Or if it's a message from a user with (NO) account
                    (newNotification.NonAccountEmail != null && x.Type == newNotification.Type && x.NonAccountEmail == newNotification.NonAccountEmail)) ||

            // If we're creating a Review notification
            (newNotification.Type == (int)NotificationType.Review && x.Type == newNotification.Type && x.ReviewId == newNotification.ReviewId) ||

            // If we're creating a Product notification
            (newNotification.Type > (int)NotificationType.Review && newNotification.Type < (int)NotificationType.Error && x.Type == newNotification.Type && x.ProductId == newNotification.ProductId))
                .Select(x => x.NotificationGroup).FirstOrDefaultAsync();





            // If a notification group does (NOT) exists
            // OR the notification type we're creating is an Error notification
            if (notificationGroup == null || newNotification.Type == (int)NotificationType.Error)
            {
                // Then create a new notification group
                notificationGroup = new NotificationGroup();
                context.NotificationGroups.Add(notificationGroup);
                await context.SaveChangesAsync();
            }


            // Now create the new notification
            var notification = new Notification()
            {
                NotificationGroupId = notificationGroup.Id,
                UserId = userId,
                ProductId = newNotification.ProductId,
                ReviewId = newNotification.ReviewId,
                Type = newNotification.Type,
                UserName = newNotification.UserName,
                UserImage = newNotification.UserImage,
                Text = newNotification.Text,
                NonAccountName = newNotification.NonAccountName,
                NonAccountEmail = newNotification.NonAccountEmail,
                CreationDate = DateTime.Now
            };
            context.Notifications.Add(notification);


            // Save
            await context.SaveChangesAsync();
        }
    }
}