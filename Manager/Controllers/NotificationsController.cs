using System;
using System.Threading.Tasks;
using DataAccess.Models;
using Manager.Classes;
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
        [Route("New")]
        public async Task<ActionResult> GetNewNotifications()
        {
            return Ok(await unitOfWork.Notifications.GetNewNotifications());
        }



        //[HttpGet]
        //[Route("Archive")]
        //public async Task<ActionResult> GetArchive()
        //{
        //    return Ok(await unitOfWork.Notifications.GetCollection<NotificationListItemViewModel>(x => x.State == 2));
        //}





        [HttpGet]
        [Route("Message")]
        public async Task<ActionResult> GetMessageNotification(int type, int state, string email)
        {
            return Ok(await unitOfWork.Notifications.GetMessageNotification(type, state, email));
        }





        [HttpGet]
        [Route("ReviewComplaint")]
        public async Task<ActionResult> GetReviewComplaintNotification(int productId, int type, int state)
        {


            
            return Ok(await unitOfWork.Notifications.GetReviewComplaintNotification(productId, type, state));
        }




        [HttpGet]
        [Route("Product")]
        public async Task<ActionResult> GetProductNotification(int productId, int type, int state)
        {
            return Ok(await unitOfWork.Notifications.GetProductNotification(productId, type, state));
        }





        







        [HttpPut]
        [Route("State")]
        public async Task<ActionResult> UpdateState(UpdatedNotification updatedNotification)
        {
            var notification = await unitOfWork.Notifications.Get(updatedNotification.Id);

            notification.State = updatedNotification.DestinationState;

            unitOfWork.Notifications.Update(notification);

            //Save
            await unitOfWork.Save();




            return Ok();
        }








        [HttpPost]
        [Route("NewNote")]
        public async Task<ActionResult> NewNote(UpdatedNotificationNotes updatedNotificationNotes)
        {
            NotificationText newNote = new NotificationText
            {
                CustomerId = "FF48C7E8FD",
                NotificationId = updatedNotificationNotes.NotificationId,
                TimeStamp = DateTime.Now,
                Text = updatedNotificationNotes.NotificationNote,
                Type = 1
            };

            unitOfWork.NotificationText.Add(newNote);

            await unitOfWork.Save();

            return Ok();
        }







        [HttpPut]
        [Route("UpdateNote")]
        public async Task<ActionResult> UpdateNote(UpdatedNotificationNotes updatedNotificationNotes)
        {

            NotificationText updatedNote = await unitOfWork.NotificationText.Get(x => x.NotificationId == updatedNotificationNotes.NotificationId && x.Type == 1);

            updatedNote.Text = updatedNotificationNotes.NotificationNote;

            unitOfWork.NotificationText.Update(updatedNote);

            await unitOfWork.Save();

            return Ok();
        }

















    }
}
