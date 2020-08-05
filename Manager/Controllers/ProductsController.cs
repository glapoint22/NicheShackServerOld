using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        [HttpGet]
        public async Task<ActionResult> GetProducts(int nicheId)
        {
          return Ok(await unitOfWork.Products.GetCollection<ItemViewModel<Product>>(x => x.NicheId == nicheId));
        }



        [HttpGet]
        [Route("Filters")]
        public async Task<ActionResult> GetFilters(int productId, int filterId)
        {
            return Ok(await unitOfWork.Products.GetProductFilters(productId, filterId));
        }


        [HttpGet]
        [Route("Product")]
        public async Task<ActionResult> GetProduct(int productId)
        {
            return Ok(await unitOfWork.Products.GetProduct(productId));
        }


        [HttpGet]
        [Route("EmailIds")]
        public async Task<ActionResult> GetEmailIds(int productId)
        {
            return Ok(await unitOfWork.ProductEmails.GetCollection(x => x.ProductId == productId, x => x.Id));
        }


        [HttpGet]
        [Route("Email")]
        public async Task<ActionResult> GetEmails(int emailId)
        {
            return Ok(await unitOfWork.ProductEmails.Get(x => x.Id == emailId, x => x.Content));
        }
    }
}
