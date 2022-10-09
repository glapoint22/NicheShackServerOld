using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Classes.Notifications;
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
        [Route("Post")]
        [Authorize(Policy = "Account Policy")]
        public async Task PostNotification(NewNotification newNotification)
        {
           await unitOfWork.Notifications.CreateNotification(newNotification, User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }




        [HttpPost]
        [Route("Message")]
        public async Task PostMessage(NewNotification newNotification)
        {
            await unitOfWork.Notifications.CreateNotification(newNotification, User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
