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
            NotificationGroup notificationGroup = await unitOfWork.NotificationGroups.Get(x => x.Id == archiveNotification.NotificationGroupId);

            notificationGroup.ArchiveDate = archiveDate;

            unitOfWork.NotificationGroups.Update(notificationGroup);
            await unitOfWork.Save();
        }
    }
}
