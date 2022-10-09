using System;
using System.Collections.Generic;
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
        [Route("Count")]
        public async Task<ActionResult> GetNotificationCount(int currentCount)
        {
            var newCount = await unitOfWork.Notifications.GetCount(x =>

            // Count all the notifications that (DO NOT) belong to an archived group
            x.NotificationGroup.ArchiveDate == null ||


            // but if it's a UserName, UserImage, or a Message that (DOES) belong to an
            // archive group and that notification has (NOT) been archived, then count that one too
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
        public async Task<ActionResult> GetNotifications(bool isNew)
        {
            return Ok(await unitOfWork.Notifications.GetNotifications(isNew));
        }





        [HttpGet]
        [Route("UserName")]
        public async Task<ActionResult> GetUserNameNotification(int notificationGroupId, bool isNew)
        {
            return Ok(await unitOfWork.Notifications.GetUserNameNotification(notificationGroupId, isNew));
        }





        [HttpGet]
        [Route("UserImage")]
        public async Task<ActionResult> GetUserImageNotification(int notificationGroupId, bool isNew)
        {
            return Ok(await unitOfWork.Notifications.GetUserImageNotification(notificationGroupId, isNew));
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
        [Route("Error")]
        public async Task<ActionResult> GetErrorNotification(int notificationGroupId)
        {
            return Ok(await unitOfWork.Notifications.GetErrorNotification(notificationGroupId));
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
            var notificationCount = 0;
            NotificationGroup notificationGroup = await unitOfWork.NotificationGroups.Get(archiveNotification.NotificationGroupId);


            // ------------ Notification Group ------------ \\

            // If the notification group exists
            // (It's possible that it could be deleted)
            if (notificationGroup != null)
            {
                // If a UserName, UserImage, or Message needs to be archived
                if (!archiveNotification.Restore)
                {
                    // Stamp its notification group with an archive date (or update it with a new date)
                    notificationGroup.ArchiveDate = DateTime.Now;
                }

                // But if a notification needs to be restored
                else
                {
                    // If just a single message is getting restored (NOT a message notification)
                    if (archiveNotification.NotificationId > 0)
                    {
                        // Check to see if there are other messages from the same sender that are still in archive
                        notificationCount = await unitOfWork.Notifications.GetCount(x => x.NotificationGroupId == archiveNotification.NotificationGroupId && x.IsArchived);
                    }

                    // Remove the archive date from its notification group as long as we're not restoring a single message
                    // from a message notification and that the message notification doesn't still have other messages in archive
                    if (notificationCount <= 1)
                    {
                        notificationGroup.ArchiveDate = null;
                    }
                }

                // Update the notification group
                unitOfWork.NotificationGroups.Update(notificationGroup);
            }



            // ------------ Archive/Restore One ------------ \\

            // If just one UserName, UserImage, or Message is getting archived
            if (archiveNotification.NotificationId > 0 && !archiveNotification.Restore ||
                // OR if just one message is getting restored
                archiveNotification.NotificationId > 0 && archiveNotification.Restore)
            {
                // Find the UserName, UserImage, or Message notification
                Notification notification = await unitOfWork.Notifications.Get(archiveNotification.NotificationId);

                // If that UserName, UserImage, or Message notification was found
                // (It's possible that it could be deleted)
                if (notification != null)
                {
                    // Set its archived state accordingly
                    notification.IsArchived = !archiveNotification.Restore;
                    unitOfWork.Notifications.Update(notification);
                }
            }



            // ------------ Archive/Restore All ------------ \\

            // If every UserName, UserImage, or Message in the group are getting archived
            if (archiveNotification.ArchiveAllMessagesInGroup ||
                // OR if all messages in the group are getting restored
                archiveNotification.RestoreAllMessagesInGroup)
            {
                // Get all notifications in the group
                IEnumerable<Notification> notifications = await unitOfWork.Notifications.GetCollection(x => x.NotificationGroupId == archiveNotification.NotificationGroupId);

                // Set each notification accordingly
                foreach (var messageNotification in notifications)
                {
                    messageNotification.IsArchived = archiveNotification.ArchiveAllMessagesInGroup;
                }
                unitOfWork.Notifications.UpdateRange(notifications);
            }

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
        [Route("BlockNotificationSending")]
        public async Task BlockNotificationSending(NoncompliantUser noncompliantUser)
        {
            Customer user = await unitOfWork.Customers.Get(noncompliantUser.UserId);
            user.BlockNotificationSending = !user.BlockNotificationSending;
            unitOfWork.Customers.Update(user);
            await unitOfWork.Save();
        }



        [HttpPost]
        [Route("BlockNonAccountEmail")]
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
        [Route("UnblockNonAccountEmail")]
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
        public async Task<ActionResult> AddNoncompliantStrike(NoncompliantUser noncompliantUser)
        {
            Customer user = await unitOfWork.Customers.Get(noncompliantUser.UserId);

            // If we're removing a name
            if (noncompliantUser.UserName != null)
            {
                bool nameRemovalSuccessful = false;

                // As long as the user's current first and last name and the name in the notification are the same
                // (They could be different because of a rare situation where the user changes their
                //  name right before the notification gets submitted)
                if ((user.FirstName + " " + user.LastName) == noncompliantUser.UserName)
                {
                    nameRemovalSuccessful = true;
                    user.NoncompliantStrikes++;
                    user.FirstName = "NicheShack";
                    user.LastName = "User";
                    unitOfWork.Customers.Update(user);
                    await unitOfWork.Save();
                }
                return Ok(nameRemovalSuccessful);


            }// If we're removing an image
            else if (noncompliantUser.UserImage != null)
            {
                bool imageRemovalSuccessful = false;

                // As long as the user's current image and the image in the notification are the same
                // (They could be different because of a rare situation where the user changes their
                //  image right before the notification gets submitted)
                if (user.Image == noncompliantUser.UserImage)
                {
                    imageRemovalSuccessful = true;
                    user.NoncompliantStrikes++;
                    user.Image = null;
                    unitOfWork.Customers.Update(user);
                    await unitOfWork.Save();
                }
                return Ok(imageRemovalSuccessful);


            }// If we're removing a review
            else
            {
                user.NoncompliantStrikes++;
                unitOfWork.Customers.Update(user);
                await unitOfWork.Save();
                return Ok();
            }
        }





        [HttpPost]
        [Route("Post")]
        public async Task<ActionResult> PostNotification(NewNotification newNotification)
        {
            // First, check to see if a notification group for the type of notification that we're going to create already exists
            NotificationGroup notificationGroup = await unitOfWork.Notifications.Get(x => x.UserId == newNotification.UserId && x.Type == newNotification.Type, x => x.NotificationGroup);

            // If a notification group does (NOT) exists
            if (notificationGroup == null)
            {
                // Create a new notification group
                notificationGroup = new NotificationGroup
                {
                    ArchiveDate = DateTime.Now
                };
                unitOfWork.NotificationGroups.Add(notificationGroup);
                await unitOfWork.Save();
            }

            // But if a notification group already exitsts
            else
            {
                // Update the archive date with a new date
                notificationGroup.ArchiveDate = DateTime.Now;
                unitOfWork.NotificationGroups.Update(notificationGroup);
            }
            


            // Now create the new notification
            var notification = new Notification()
            {
                NotificationGroupId = notificationGroup.Id,
                UserId = newNotification.UserId,
                Type = newNotification.Type,
                UserName = newNotification.UserName,
                UserImage = newNotification.UserImage,
                IsArchived = true,
                CreationDate = DateTime.Now
            };
            unitOfWork.Notifications.Add(notification);
            await unitOfWork.Save();


            // If notes were written
            if (newNotification.EmployeeNotes != null)
            {
                // Save the notes
                NotificationEmployeeNote notificationEmployeeNote = new NotificationEmployeeNote
                {
                    NotificationGroupId = notification.NotificationGroupId,
                    NotificationId = notification.Id,
                    EmployeeId = "835529FC9A",
                    Note = newNotification.EmployeeNotes,
                    CreationDate = DateTime.Now
                };
                unitOfWork.NotificationEmployeeNotes.Add(notificationEmployeeNote);
                await unitOfWork.Save();
            }

            // Return the notification item
            var NotificationItem = new NotificationItem
            {
                Id = notification.Id,
                NotificationGroupId = notification.NotificationGroupId,
                NotificationType = notification.Type,
                UserName = notification.UserName,
                UserImage = notification.UserImage,
                IsNew = false,
                CreationDate = notification.CreationDate,
                Name = newNotification.Type == (int)NotificationType.UserName ? "User Name" : "User Image"
            };
            return Ok(NotificationItem);
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
                    // Then delete only the notifications that contains the ids that are in the 'notificationIds' list
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