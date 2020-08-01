using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase {
        private readonly IUnitOfWork unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ActionResult> GetProducts(int nicheId)
        {
          return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(x => x.NicheId == nicheId));
        }
    }
}
