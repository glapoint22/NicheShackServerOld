using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;
using Manager.Classes;
using Manager.Repositories;
using Manager.ViewModels;
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
        [Route("Notification")]
        public async Task<ActionResult> Get(int id)
        {
            object notification = null;
            NotificationType type = (NotificationType)await unitOfWork.Notifications.Get(x => x.Id == id, x => x.Type);


            // Message
            if (type == NotificationType.Message)
            {
                notification = await unitOfWork.Notifications.Get<NotificationViewModel>(x => x.Id == id);
            }

            // General Notification
            if (type == NotificationType.ProductNameOther
              || type == NotificationType.ProductReportedAsIllegal
              || type == NotificationType.ProductReportedAsHavingAdultContent
              || type == NotificationType.OffensiveProductOther
              || type == NotificationType.ProductInactive
              || type == NotificationType.ProductSiteNoLongerInService
              || type == NotificationType.MissingProductOther)
            {

                notification = await unitOfWork.Notifications.Get<GeneralNotificationViewModel>(x => x.Id == id);
            }

            // Review Complaint Notification
            if (type == NotificationType.ReviewComplaint)
            {
                notification = await unitOfWork.Notifications.Get<GeneralNotificationViewModel>(x => x.Id == id);
            }

            // Product Description Notification
            if (type == NotificationType.ProductNameDoesNotMatchWithProductDescription
              || type == NotificationType.ProductDescriptionIncorrect
              || type == NotificationType.ProductDescriptionTooVague
              || type == NotificationType.ProductDescriptionMisleading
              || type == NotificationType.ProductDescriptionOther)
            {
                notification = await unitOfWork.Notifications.Get<ProductDescriptionNotificationViewModel>(x => x.Id == id);
            }

            // Product Image Notification
            if (type == NotificationType.ProductNameDoesNotMatchWithProductImage)
            {
                notification = await unitOfWork.Notifications.Get<ProductImageNotificationViewModel>(x => x.Id == id);
            }

            // Product Media Notification
            if (type == NotificationType.VideosAndImagesAreDifferentFromProduct
              || type == NotificationType.NotEnoughVideosAndImages
              || type == NotificationType.VideosAndImagesNotClear
              || type == NotificationType.VideosAndImagesMisleading
              || type == NotificationType.VideosAndImagesOther)
            {
                notification = await unitOfWork.Notifications.Get<ProductMediaNotificationViewModel>(x => x.Id == id);
            }

            // Product Content Notification
            if (type == NotificationType.ProductPriceTooHigh
              || type == NotificationType.ProductPriceNotCorrect
              || type == NotificationType.ProductPriceOther)
            {

                notification = await unitOfWork.Notifications.Get<ProductContentNotificationViewModel>(x => x.Id == id);
            }


            return Ok(notification);
        }




        



        [HttpGet]
        [Route("Load")]
        public async Task<ActionResult> Load()
        {
            return Ok(await unitOfWork.Notifications.GetCollection<NotificationListItemViewModel>(x => x.State == 0 || x.State == 1));
        }






        [HttpGet]
        [Route("Archive")]
        public async Task<ActionResult> GetArchive()
        {
            return Ok(await unitOfWork.Notifications.GetCollection<NotificationListItemViewModel>(x => x.State == 2));
        }




        [HttpPost]
        [Route("Ids")]
        public async Task<ActionResult> GetIds(NotificationListItemViewModel notificationListItem)
        {

            return Ok(await unitOfWork.Notifications.GetCollection(x => 
                x.ProductId == notificationListItem.ProductId &&
                x.Type == notificationListItem.Type &&
                x.State == notificationListItem.State, x => x.Id));
        }





       

        [HttpPut]
        [Route("State")]
        public async Task<ActionResult> UpdateState(UpdatedNotification updatedNotification)
        {
            IEnumerable<Notification> notifications = null;

            if (updatedNotification.ProductId == 0)
            {
                notifications = await unitOfWork.Notifications.GetCollection(x => x.Type == updatedNotification.Type && x.State == updatedNotification.CurrentState);
            }else
            {
                notifications = await unitOfWork.Notifications.GetCollection(x => x.ProductId == updatedNotification.ProductId && x.Type == updatedNotification.Type && x.State == updatedNotification.CurrentState);
            }
            


            foreach (Notification notification in notifications)
            {

                notification.State = updatedNotification.DestinationState;

                unitOfWork.Notifications.Update(notification);
            }

            //Save
            await unitOfWork.Save();




            return Ok();
        }







    }
}
