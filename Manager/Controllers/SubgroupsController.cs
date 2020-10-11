using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubgroupsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public SubgroupsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetSubgroups()
        {
            return Ok(await unitOfWork.Subgroups.GetCollection<ItemViewModel<Subgroup>>());
        }
    }
}
