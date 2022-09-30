using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Manager.Classes.Notifications;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using static Manager.Classes.Utility;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public NotificationsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }





        [HttpGet]
        public async Task<ActionResult> GetNotifications(bool isNew)
        {
            return Ok(await unitOfWork.Notifications.GetNotifications(isNew));
        }





        [HttpGet]
        [Route("Count")]
        public async Task<ActionResult> GetNotificationCount(int currentCount)
        {
            var newCount = await unitOfWork.Notifications.GetCount(x =>

            // Count all the notifications that DO NOT belong to an archived group
            x.NotificationGroup.ArchiveDate == null ||


            // but if it's a UserName, UserImage, or a Message that does belong to an
            // archive group and that notification has NOT been archived, then count that one too
            (x.Type == (int)NotificationType.UserName ||
            x.Type == (int)NotificationType.UserImage ||
            x.Type == (int)NotificationType.Message) &&
            !x.IsArchived);


            if (currentCount != newCount)
            {
                var notifications = await unitOfWork.Notifications.GetNotifications(true);
                return Ok(new { Count = newCount, Notifications = notifications });
            }
            else
            {
                return Ok();
            }
        }




        [HttpGet]
        [Route("Message")]
        public async Task<ActionResult> GetMessageNotification(int notificationGroupId, bool isNew)
        {
            return Ok(await unitOfWork.Notifications.GetMessageNotification(notificationGroupId, isNew));
        }





        [HttpGet]
        [Route("Review")]
        public async Task<ActionResult> GetReviewNotification(int notificationGroupId)
        {
            return Ok(await unitOfWork.Notifications.GetReviewNotification(notificationGroupId));
        }




        [HttpGet]
        [Route("Product")]
        public async Task<ActionResult> GetProductNotification(int notificationGroupId)
        {
            return Ok(await unitOfWork.Notifications.GetProductNotification(notificationGroupId));
        }




        [HttpGet]
        [Route("UserImage")]
        public async Task<ActionResult> GetUserImageNotification(int notificationGroupId, bool isNew)
        {
            return Ok(await unitOfWork.Notifications.GetUserImageNotification(notificationGroupId, isNew));
        }






        [HttpPost]
        [Route("PostNote")]
        public async Task PostNote(NewNotificationEmployeeNote newNotificationEmployeeNote)
        {
            // Post the new note
            NotificationEmployeeNote notificationEmployeeNote = new NotificationEmployeeNote
            {
                NotificationGroupId = newNotificationEmployeeNote.NotificationGroupId,
                NotificationId = newNotificationEmployeeNote.NotificationId > 0 ? newNotificationEmployeeNote.NotificationId : null,
                EmployeeId = "835529FC9A",
                Note = newNotificationEmployeeNote.Note,
                CreationDate = DateTime.Now
            };
            unitOfWork.NotificationEmployeeNotes.Add(notificationEmployeeNote);
            await unitOfWork.Save();
        }











        [HttpPut]
        [Route("Archive")]
        public async Task ArchiveNotification(ArchiveNotification archiveNotification)
        {
            var messageCount = 0;
            NotificationGroup notificationGroup = await unitOfWork.NotificationGroups.Get(archiveNotification.NotificationGroupId);





            // ------------ Notification Group ------------ \\

            // If a notification needs to be archived
            if (!archiveNotification.Restore)
            {
                // Stamp its notification group with an archive date
                notificationGroup.ArchiveDate = DateTime.Now;
            }

            // But if a notification needs to be restored
            else
            {
                // If just a single message is getting restored (NOT a message notification)
                if (archiveNotification.NotificationId > 0)
                {
                    // Check to see if there are other messages from the same sender that are still in archive
                    messageCount = await unitOfWork.Notifications.GetCount(x => x.NotificationGroupId == archiveNotification.NotificationGroupId && x.IsArchived);
                }

                // Remove the archive date from its notification group as long as we're not restoring a single message
                // from a message notification and that message notification doesn't still have other messages in archive
                if (messageCount <= 1)
                {
                    notificationGroup.ArchiveDate = null;
                }
            }





            // ------------ One Message ------------ \\

            // If just one message is getting archived
            if (archiveNotification.NotificationId > 0 && !archiveNotification.Restore ||
                // OR if just one message is getting restored
                archiveNotification.NotificationId > 0 && archiveNotification.Restore)
            {
                // Mark the message accordingly
                Notification notification = await unitOfWork.Notifications.Get(archiveNotification.NotificationId);
                notification.IsArchived = !archiveNotification.Restore;
                unitOfWork.Notifications.Update(notification);
            }






            // ------------ All Messages In Group ------------ \\

            // If all messages in a group are getting archived
            if (archiveNotification.ArchiveAllMessagesInGroup ||
                // OR if all messages in a group are getting restored
                archiveNotification.RestoreAllMessagesInGroup)
            {
                // Get all messages in the group
                IEnumerable<Notification> messageNotifications = await unitOfWork.Notifications.GetCollection(x => x.NotificationGroupId == archiveNotification.NotificationGroupId);

                // Mark each message accordingly
                foreach (var messageNotification in messageNotifications)
                {
                    messageNotification.IsArchived = archiveNotification.ArchiveAllMessagesInGroup;
                }
                unitOfWork.Notifications.UpdateRange(messageNotifications);
            }




            // Update the notification group
            unitOfWork.NotificationGroups.Update(notificationGroup);
            await unitOfWork.Save();
        }









        [HttpPut]
        [Route("RemoveReview")]
        public async Task RemoveReview(RemoveReview removeReview)
        {
            ProductReview review = await unitOfWork.ProductReviews.Get(removeReview.ReviewId);
            review.Deleted = !review.Deleted;
            unitOfWork.ProductReviews.Update(review);
            await unitOfWork.Save();
        }




        [HttpPut]
        [Route("DisableProduct")]
        public async Task DisableProduct(DisableProduct disableProduct)
        {
            Product product = await unitOfWork.Products.Get(disableProduct.ProductId);
            product.Disabled = !product.Disabled;
            unitOfWork.Products.Update(product);
            await unitOfWork.Save();
        }




        [HttpPut]
        [Route("NotificationSending")]
        public async Task NotificationSending(NoncompliantUser noncompliantUser)
        {
            Customer user = await unitOfWork.Customers.Get(noncompliantUser.UserId);
            user.BlockNotificationSending = !user.BlockNotificationSending;
            unitOfWork.Customers.Update(user);
            await unitOfWork.Save();
        }



        [HttpPost]
        [Route("BlockEmail")]
        public async Task BlockEmail(NoncompliantUser noncompliantUser)
        {
            var email = await unitOfWork.BlockedNonAccountEmails.Get(x => x.Email == noncompliantUser.Email);

            if (email == null)
            {
                var newBlockedEmail = new BlockedNonAccountEmail
                {
                    Email = noncompliantUser.Email
                };

                unitOfWork.BlockedNonAccountEmails.Add(newBlockedEmail);
                await unitOfWork.Save();
            }
        }




        [HttpDelete]
        [Route("UnblockEmail")]
        public async Task UnblockEmail(string blockedEmail)
        {
            var email = await unitOfWork.BlockedNonAccountEmails.Get(x => x.Email == blockedEmail);

            if (email != null)
            {
                unitOfWork.BlockedNonAccountEmails.Remove(email);
                await unitOfWork.Save();
            }
        }




        [HttpPut]
        [Route("AddNoncompliantStrike")]
        public async Task AddNoncompliantStrike(NoncompliantUser noncompliantUser)
        {
            Customer user = await unitOfWork.Customers.Get(noncompliantUser.UserId);
            user.NoncompliantStrikes++;
            if (noncompliantUser.RemoveProfilePic) user.Image = null;
            unitOfWork.Customers.Update(user);
            await unitOfWork.Save();
        }



        [HttpDelete]
        public async Task DeleteNotification(int notificationGroupId, [FromQuery] List<int?> notificationIds)
        {
            var notificationCount = await unitOfWork.Notifications.GetCount(x => x.NotificationGroupId == notificationGroupId);
            var employeeNotes = await unitOfWork.NotificationEmployeeNotes.GetCollection(x => notificationIds.Count > 0 ? notificationIds.Contains(x.NotificationId) : x.NotificationGroupId == notificationGroupId);


            // If we're deleting a UserName, UserImage, or a Message
            if (notificationIds.Count > 0)
            {
                // And (NOT) every notification in the group is getting deleted
                // (i.e. Some notifications in the group still remain in the NEW list or only one Message in the group is getting deleted)
                if (notificationCount > notificationIds.Count)
                {
                    // Then only delete the notifications that contains the ids that are in the 'notificationIds' list
                    var notifications = await unitOfWork.Notifications.GetCollection(x => notificationIds.Contains(x.Id));
                    unitOfWork.Notifications.RemoveRange(notifications);
                }
            }

            // But if we're (NOT) deleting a UserName, UserImage, and a Message
            if (notificationIds.Count == 0
               // Or we (ARE) deleting a UserName, UserImage, or a Message, but we're deleting every notification in its group
               || (notificationIds.Count > 0 && notificationCount == notificationIds.Count))
            {
                // Then just remove the entire group
                var notificationGroup = await unitOfWork.NotificationGroups.Get(notificationGroupId);
                unitOfWork.NotificationGroups.Remove(notificationGroup);
            }

            // Remove the employee notes (if any)
            unitOfWork.NotificationEmployeeNotes.RemoveRange(employeeNotes);

            // Save
            await unitOfWork.Save();
        }
    }
}