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
        [Route("Message")]
        public async Task<ActionResult> GetMessageNotification(string email, int type, DateTime? archiveDate)
        {
            return Ok(await unitOfWork.Notifications.GetMessageNotification(email, type, archiveDate));
        }





        [HttpGet]
        [Route("Review")]
        public async Task<ActionResult> GetReviewNotification(int productId, int type, DateTime? archiveDate)
        {
            return Ok(await unitOfWork.Notifications.GetReviewNotification(productId, type, archiveDate));
        }




        [HttpGet]
        [Route("Product")]
        public async Task<ActionResult> GetProductNotification(int productId, int type, DateTime? archiveDate)
        {
            return Ok(await unitOfWork.Notifications.GetProductNotification(productId, type, archiveDate));
        }





        
        [HttpPost]
        [Route("PostNote")]
        public async Task PostNote(NewNotificationEmployeeNote newNotificationEmployeeNote)
        {
            int notificationId = await unitOfWork.Notifications.Get(x => x.ProductId == newNotificationEmployeeNote.ProductId &&
                                                                         x.Type == newNotificationEmployeeNote.NotificationType &&
                                                                         x.ArchiveDate == newNotificationEmployeeNote.ArchiveDate,
                                                                    x => x.Id);

            // Post the new note
            NotificationEmployeeNote notificationEmployeeNote = new NotificationEmployeeNote
            {
                EmployeeId = "835529FC9A",
                TimeStamp = DateTime.Now,
                Text = newNotificationEmployeeNote.Text
            };
            unitOfWork.NotificationEmployeeNotes.Add(notificationEmployeeNote);
            await unitOfWork.Save();

            // Store the id of the new posted note in the NotificationDetails table so that the associated notification has reference to it 
            NotificationDetails notificationDetails = await unitOfWork.NotificationDetails.Get(x => x.NotificationId == notificationId);
            notificationDetails.NotificationEmployeeNoteId = notificationEmployeeNote.Id;
            unitOfWork.NotificationDetails.Update(notificationDetails);
            await unitOfWork.Save();
        }






        [HttpPost]
        [Route("PostReply")]
        public async Task PostReply(NewNotificationEmployeeNote newNotificationMessageReply)
        {
            int notificationId = await unitOfWork.Notifications.Get(x => x.Id == newNotificationMessageReply.MessageId &&
                                                                        (x.NotificationDetails.Select(y => y.Customer).FirstOrDefault() == null ?
                                                                            (x.NotificationDetails.Select(y => y.Email).FirstOrDefault() == newNotificationMessageReply.Email) :
                                                                             x.NotificationDetails.Select(y => y.Customer.Email).FirstOrDefault() == newNotificationMessageReply.Email) &&
                                                                         x.Type == newNotificationMessageReply.NotificationType &&
                                                                         x.ArchiveDate == newNotificationMessageReply.ArchiveDate,
                                                                    x => x.Id);

            // Post the new message reply
            NotificationEmployeeNote notificationMessageReply = new NotificationEmployeeNote
            {
                EmployeeId = "835529FC9A",
                TimeStamp = DateTime.Now,
                Text = newNotificationMessageReply.Text
            };
            unitOfWork.NotificationEmployeeNotes.Add(notificationMessageReply);
            await unitOfWork.Save();

            // Store the id of the new message reply in the NotificationDetails table so that the associated notification has reference to it 
            NotificationDetails notificationDetails = await unitOfWork.NotificationDetails.Get(x => x.NotificationId == notificationId);
            notificationDetails.NotificationEmployeeNoteId = notificationMessageReply.Id;
            unitOfWork.NotificationDetails.Update(notificationDetails);
            await unitOfWork.Save();
        }







        [HttpPut]
        [Route("Archive")]
        public async Task ArchiveNotification(ArchiveNotification archiveNotification)
        {
            DateTime archiveDate = DateTime.Now;
            IEnumerable<Notification> notifications = await unitOfWork.Notifications.GetCollection(x => x.ProductId == archiveNotification.ProductId &&
                                                                                                        x.Type == archiveNotification.NotificationType &&
                                                                                                        x.ArchiveDate == null);

            foreach (Notification notification in notifications)
            {
                notification.ArchiveDate = archiveDate;
            }
            unitOfWork.Notifications.UpdateRange(notifications);
            await unitOfWork.Save();
        }




        [HttpPut]
        [Route("Message/Archive")]
        public async Task ArchiveMessageNotification(ArchiveNotification archiveNotification)
        {
            DateTime archiveDate = DateTime.Now;
            IEnumerable<Notification> notifications = await unitOfWork.Notifications.GetCollection(x => (x.NotificationDetails.Select(y => y.Customer).FirstOrDefault() == null ?
                                                                                                            (x.NotificationDetails.Select(y => y.Email).FirstOrDefault() == archiveNotification.Email) :
                                                                                                             x.NotificationDetails.Select(y => y.Customer.Email).FirstOrDefault() == archiveNotification.Email) &&
                                                                                                         x.Type == archiveNotification.NotificationType &&
                                                                                                         x.ArchiveDate == null);

            foreach (Notification notification in notifications)
            {
                notification.ArchiveDate = archiveDate;
            }
            unitOfWork.Notifications.UpdateRange(notifications);
            await unitOfWork.Save();
        }

    }
}
