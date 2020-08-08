using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public VendorsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetVendors()
        {
            return Ok(await unitOfWork.Vendors.GetCollection<ItemViewModel<Vendor>>());
        }


        [Route("Vendor")]
        [HttpGet]
        public async Task<ActionResult> GetVendor(int vendorId)
        {
            return Ok(await unitOfWork.Vendors.Get(vendorId));
        }




        [Route("Products")]
        [HttpGet]
        public async Task<ActionResult> GetProducts(int vendorId)
        {
            return Ok(await unitOfWork.Products.GetCollection(x => x.VendorId == vendorId, x => new { 
                x.Id,
                x.Name,
                x.Hoplink
            }));
        }



    }
}
