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
            var newCount = await unitOfWork.Notifications.GetCount(x => x.NotificationGroup.ArchiveDate == null);

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
        public async Task<ActionResult> GetMessageNotification(int notificationGroupId)
        {
            return Ok(await unitOfWork.Notifications.GetMessageNotification(notificationGroupId));
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
            DateTime archiveDate = DateTime.Now;
            NotificationGroup notificationGroup = await unitOfWork.NotificationGroups.Get(archiveNotification.NotificationGroupId);


            if (notificationGroup.ArchiveDate == null)
            {
                notificationGroup.ArchiveDate = archiveDate;
            }
            else
            {
                notificationGroup.ArchiveDate = null;
            }


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
        public async Task DeleteNotification(int notificationGroupId)
        {
            var notificationGroup = await unitOfWork.NotificationGroups.Get(notificationGroupId);

            unitOfWork.NotificationGroups.Remove(notificationGroup);
            await unitOfWork.Save();
        }
    }
}
