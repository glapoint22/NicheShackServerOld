using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Manager.Classes;
using Manager.Classes.Notifications;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            // but if it's a message notification that belongs to an archive group and
            // that message notification has NOT been archived, then count that one too
            (x.Type == 0 && !x.MessageArchived));

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






        [HttpPost]
        [Route("PostNote")]
        public async Task PostNote(NewNotificationEmployeeNote newNotificationEmployeeNote)
        {
            // Post the new note
            NotificationEmployeeNote notificationEmployeeNote = new NotificationEmployeeNote
            {
                NotificationGroupId = newNotificationEmployeeNote.NotificationGroupId,
                EmployeeId = "835529FC9A",
                Note = newNotificationEmployeeNote.Note,
                CreationDate = DateTime.Now
            };
            unitOfWork.NotificationEmployeeNotes.Add(notificationEmployeeNote);
            await unitOfWork.Save();
        }






        [HttpPost]
        [Route("PostMessage")]
        public async Task PostMessage(NewNotificationEmployeeMessage newNotificationEmployeeMessage)
        {
            // Post the new message
            NotificationEmployeeMessage notificationEmployeeMessage = new NotificationEmployeeMessage
            {
                EmployeeId = "835529FC9A",
                Message = newNotificationEmployeeMessage.Message,
                CreationDate = DateTime.Now,
            };
            unitOfWork.NotificationEmployeeMessages.Add(notificationEmployeeMessage);
            await unitOfWork.Save();

            // Store the id of the new message in the Notifications table so that the associated notification has reference to it 
            Notification notification = await unitOfWork.Notifications.Get(x => x.Id == newNotificationEmployeeMessage.NotificationId);
            notification.EmployeeMessageId = notificationEmployeeMessage.Id;
            unitOfWork.Notifications.Update(notification);
            await unitOfWork.Save();
        }







        [HttpPut]
        [Route("Archive")]
        public async Task ArchiveNotification(ArchiveNotification archiveNotification)
        {
            var messageCount = 0;
            DateTime archiveDate = DateTime.Now;
            NotificationGroup notificationGroup = await unitOfWork.NotificationGroups.Get(archiveNotification.NotificationGroupId);





            // ------------ Notification Group ------------ \\

            // If a notification needs to be archived
            if (!archiveNotification.Restore)
            {
                // Stamp its notification group with an archive date
                notificationGroup.ArchiveDate = archiveDate;
            }

            // But if a notification needs to be restored
            else
            {
                // If just a single message is getting restored (NOT a message notification)
                if (archiveNotification.NotificationId > 0)
                {
                    // Check to see if there are other messages from the same sender that are still in archive
                    messageCount = await unitOfWork.Notifications.GetCount(x => x.NotificationGroupId == archiveNotification.NotificationGroupId && x.MessageArchived);
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
                notification.MessageArchived = !archiveNotification.Restore;
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
                    messageNotification.MessageArchived = archiveNotification.ArchiveAllMessagesInGroup;
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
        public async Task DeleteNotification(int notificationGroupId, int notificationId)
        {
            var messageCount = 0;

            // If just a single message is getting deleted (NOT a message notification)
            // Or if a message notification is getting deleted but it only has one message
            if (notificationId > 0)
            {
                // Delete that message
                Notification notification = await unitOfWork.Notifications.Get(notificationId);
                unitOfWork.Notifications.Remove(notification);

                // Check to see if there are other messages from the same sender that still have NOT been deleted
                messageCount = await unitOfWork.Notifications.GetCount(x => x.NotificationGroupId == notificationGroupId && x.Id != notificationId);
            }

            // Remove the notification group as long as we're not deleting a single message from a message
            // notification and that message notification doesn't still have other messages that aren't deleted yet
            if (messageCount <= 1)
            {
                var notificationGroup = await unitOfWork.NotificationGroups.Get(notificationGroupId);
                unitOfWork.NotificationGroups.Remove(notificationGroup);
            }


            //await unitOfWork.Save();
        }
    }
}
