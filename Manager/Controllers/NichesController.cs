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
    }
}
