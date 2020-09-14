using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using Website.Repositories;

namespace Website.Controllers
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


        [HttpPost]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> PostNotification(NotificationData notificationData)
        {
            Notification notification = new Notification
            {
                ProductId = notificationData.ProductId,
                Type = notificationData.Type,
                State = 0
            };

            unitOfWork.Notifications.Add(notification);


            
            await unitOfWork.Save();

            NotificationText notificationText = new NotificationText
            {
                CustomerId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                NotificationId = notification.Id,
                TimeStamp = DateTime.Now,
                Type = 0,
                Text = notificationData.Comments
            };


            unitOfWork.NotificationText.Add(notificationText);
            await unitOfWork.Save();

            return Ok();
        }


        [HttpPost]
        [Route("Message")]
        public async Task<ActionResult> PostMessage(MessageNotification messageNotification)
        {
            Notification notification = new Notification
            {
                ProductId = null,
                Type = 0,
                State = 0
            };

            unitOfWork.Notifications.Add(notification);

            await unitOfWork.Save();

            NotificationText notificationText = new NotificationText
            {
                CustomerId = User.FindFirst(ClaimTypes.NameIdentifier) != null ? User.FindFirst(ClaimTypes.NameIdentifier).Value : null,
                NotificationId = notification.Id,
                TimeStamp = DateTime.Now,
                Type = 0,
                Text = messageNotification.Message,
                Email = messageNotification.Email,
                Name = messageNotification.Name
            };


            unitOfWork.NotificationText.Add(notificationText);
            await unitOfWork.Save();

            return Ok();
        }
    }
}
