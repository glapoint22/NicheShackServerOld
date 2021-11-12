using System.Threading.Tasks;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Website.Repositories;
using Website.ViewModels;



namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NichesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public NichesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        // ..................................................................................Get Niches.....................................................................
        [HttpGet]
        public async Task<ActionResult> GetNiches(int id)
        {
            // Get all categories and their niches
            return Ok(await unitOfWork.Niches.GetCollection<UrlItemViewModel<Niche>>(x => x.CategoryId == id));
        }
    }
}
