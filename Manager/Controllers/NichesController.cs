using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NichesController : ControllerBase {
        private readonly IUnitOfWork unitOfWork;

        public NichesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetNiches(int categoryId)
        {
            return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(x => x.CategoryId == categoryId));
        }


        [Route("LeadPageIds")]  
        [HttpGet]
        public async Task<ActionResult> GetLeadpageIds(int nicheId)
        {
            return Ok(await unitOfWork.LeadPages.GetCollection(x => x.NicheId == nicheId, x => x.Id));
        }


        [Route("LeadPages")]
        [HttpGet]
        public async Task<ActionResult> GetLeadpages(int leadPageId)
        {
            return Ok(await unitOfWork.LeadPages.GetCollection(x => x.Id == leadPageId, x => x.Content));
        }


        [Route("LeadPageEmails")]
        [HttpGet]
        public async Task<ActionResult> GetLeadpageEmails(int leadPageId)
        {
            return Ok(await unitOfWork.LeadPageEmails.GetCollection(x => x.Id == leadPageId, x => x.Content));
        }
    }
}
