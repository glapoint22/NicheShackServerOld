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
    public class EmailPreferencesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public EmailPreferencesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // ..................................................................................Get Preferences.....................................................................
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetPreferences()
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var preferences = await unitOfWork.Customers.Get(x => x.Id == customerId, x => new EmailPreferences
            {
                NameChange = x.EmailPrefNameChange,
                EmailChange = x.EmailPrefEmailChange,
                PasswordChange = x.EmailPrefPasswordChange,
                ProfilePicChange = x.EmailPrefProfilePicChange,
                NewCollaborator = x.EmailPrefNewCollaborator,
                RemovedCollaborator = x.EmailPrefRemovedCollaborator,
                RemovedListItem = x.EmailPrefRemovedListItem,
                MovedListItem = x.EmailPrefMovedListItem,
                AddedListItem = x.EmailPrefAddedListItem,
                ListNameChange = x.EmailPrefListNameChange,
                DeletedList = x.EmailPrefDeletedList,
                Review = x.EmailPrefReview
            });


            return Ok(preferences);
        }






        // ..................................................................................Set Preferences.....................................................................
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> SetPreferences(EmailPreferences emailPreferences)
        {

            Customer customer = await unitOfWork.Customers.Get(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            customer.EmailPrefNameChange = emailPreferences.NameChange;
            customer.EmailPrefEmailChange = emailPreferences.EmailChange;
            customer.EmailPrefPasswordChange = emailPreferences.PasswordChange;
            customer.EmailPrefProfilePicChange = emailPreferences.ProfilePicChange;
            customer.EmailPrefNewCollaborator = emailPreferences.NewCollaborator;
            customer.EmailPrefRemovedCollaborator = emailPreferences.RemovedCollaborator;
            customer.EmailPrefRemovedListItem = emailPreferences.RemovedListItem;
            customer.EmailPrefMovedListItem = emailPreferences.MovedListItem;
            customer.EmailPrefAddedListItem = emailPreferences.AddedListItem;
            customer.EmailPrefListNameChange = emailPreferences.ListNameChange;
            customer.EmailPrefDeletedList = emailPreferences.DeletedList;
            customer.EmailPrefReview = emailPreferences.Review;


            await unitOfWork.Save();

            return Ok();
        }
    }
}
