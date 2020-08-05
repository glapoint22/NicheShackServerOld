using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using Manager.Interfaces;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Http;
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
        [Route("Notification")]
        public async Task<ActionResult> Get(int id, int state)
        {
            int type = await unitOfWork.Notifications.Get(x => x.Id == id, x => x.Type);

            INotification notification = null;


            switch (type)
            {
                case 0:
                    notification = await unitOfWork.Notifications.Get<NotificationViewModel>(x => x.Id == id && x.State == state);
                    break;

                case 1:
                    notification = await unitOfWork.Notifications.Get<GeneralNotificationViewModel>(x => x.Id == id && x.State == state);
                    break;

                case 2:
                    notification = await unitOfWork.Notifications.Get<ProductContentNotificationViewModel>(x => x.Id == id && x.State == state);
                    break;

                case 3:
                    notification = await unitOfWork.Notifications.Get<ProductDescriptionNotificationViewModel>(x => x.Id == id && x.State == state);
                    break;

                case 4:
                    notification = await unitOfWork.Notifications.Get<ProductImageNotificationViewModel>(x => x.Id == id && x.State == state);
                    break;

                case 5:
                    notification = await unitOfWork.Notifications.Get<ProductMediaNotificationViewModel>(x => x.Id == id && x.State == state);
                    break;
            }

            return Ok(notification);

        }




        [HttpGet]
        [Route("Count")]
        public async Task<ActionResult> GetCount(int state)
        {
            return Ok(await unitOfWork.Notifications.GetCount(x => x.State == state));
        }





        [HttpGet]
        [Route("List")]
        public async Task<ActionResult> GetList(int state)
        {
            return Ok(await unitOfWork.Notifications.GetCollection(x => x.State == state, x => new
            {
                x.Id,
                Name = GetName(x.Type),
                listIcon = x.Type > 1 ? x.Product.Media.Url : x.Type == 0 ? "message.png" : "review-complaint.png"
            }));
        }




        [HttpGet]
        [Route("Ids")]
        public async Task<ActionResult> GetIds(int type, int state)
        {
            return Ok(await unitOfWork.Notifications.GetCollection(x => x.Type == type && x.State == state, x => x.Id));
        }





        private static string GetName(int type)
        {
            string name = string.Empty;

            switch (type)
            {
                case 0:
                    name = "Message";
                    break;

                case 1:
                    name = "Review Complaint";
                    break;

                case 2:
                    name = "Product Name Doesn\'t Match With Product Description";
                    break;

                case 3:
                    name = "Product Name Doesn\'t Match With Product Image";
                    break;

                case 4:
                    name = "Product Name (Other)";
                    break;

                case 5:
                    name = "Product Price Too High";
                    break;

                case 6:
                    name = "Product Price Not Correct";
                    break;

                case 7:
                    name = "Product Price (Other)";
                    break;

                case 8:
                    name = "Videos & Images are Different From Product";
                    break;

                case 9:
                    name = "Not Enough Videos & Images";
                    break;

                case 10:
                    name = "Videos & Images Not Clear";
                    break;

                case 11:
                    name = "Videos & Images Misleading";
                    break;

                case 12:
                    name = "Videos & Images (Other)";
                    break;

                case 13:
                    name = "Product Description Incorrect";
                    break;

                case 14:
                    name = "Product Description Too Vague";
                    break;

                case 15:
                    name = "Product Description Misleading";
                    break;

                case 16:
                    name = "Product Description (Other)";
                    break;

                case 17:
                    name = "Product Reported As Illegal";
                    break;

                case 18:
                    name = "Product Reported As Having Adult Content";
                    break;

                case 19:
                    name = "Offensive Product (Other)";
                    break;


                case 20:
                    name = "Product Inactive";
                    break;

                case 21:
                    name = "Product site no longer in service";
                    break;


                case 22:
                    name = "Missing Product (Other)";
                    break;
            }

            return name;
        }
    }
}
