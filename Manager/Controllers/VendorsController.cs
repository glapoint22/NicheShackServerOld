﻿using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Manager.ViewModels;
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

        // Not sure if this is needed
        //[HttpGet]
        //public async Task<ActionResult> GetVendors()
        //{
        //    return Ok(await unitOfWork.Vendors.GetCollection<ItemViewModel<Vendor>>());
        //}


        // Not sure if this is needed
        //[Route("Vendor")]
        //[HttpGet]
        //public async Task<ActionResult> GetVendor(int vendorId)
        //{
        //    return Ok(await unitOfWork.Vendors.Get(vendorId));
        //}





        [HttpPut]
        public async Task<ActionResult> UpdateVendor(Vendor updatedVendor)
        {
            Vendor vendor = await unitOfWork.Vendors.Get(updatedVendor.Id);

            vendor.Name = updatedVendor.Name;
            vendor.WebPage = updatedVendor.WebPage;
            vendor.Street = updatedVendor.Street;
            vendor.City = updatedVendor.City;
            vendor.Zip = updatedVendor.Zip;
            vendor.PoBox = updatedVendor.PoBox;
            vendor.State = updatedVendor.State;
            vendor.Country = updatedVendor.Country;
            vendor.PrimaryFirstName = updatedVendor.PrimaryFirstName;
            vendor.PrimaryLastName = updatedVendor.PrimaryLastName;
            vendor.PrimaryOfficePhone = updatedVendor.PrimaryOfficePhone;
            vendor.PrimaryMobilePhone = updatedVendor.PrimaryMobilePhone;
            vendor.PrimaryEmail = updatedVendor.PrimaryEmail;
            vendor.SecondaryFirstName = updatedVendor.SecondaryFirstName;
            vendor.SecondaryLastName = updatedVendor.SecondaryLastName;
            vendor.SecondaryOfficePhone = updatedVendor.SecondaryOfficePhone;
            vendor.SecondaryMobilePhone = updatedVendor.SecondaryMobilePhone;
            vendor.SecondaryEmail = updatedVendor.SecondaryEmail;
            vendor.Notes = updatedVendor.Notes;

            // Update and save
            unitOfWork.Vendors.Update(vendor);
            await unitOfWork.Save();


            return Ok();
        }








        [HttpPost]
        public async Task<ActionResult> AddVendor(Vendor vendor)
        {
            // Add and save
            unitOfWork.Vendors.Add(vendor);
            await unitOfWork.Save();

            return Ok(vendor.Id);
        }





        [HttpDelete]
        public async Task<ActionResult> DeleteVendor(int vendorId)
        {
            Vendor vendor = await unitOfWork.Vendors.Get(vendorId);

            unitOfWork.Vendors.Remove(vendor);
            await unitOfWork.Save();
            return Ok();
        }






        [Route("Products")]
        [HttpGet]
        public async Task<ActionResult> GetVendorProducts(int vendorId)
        {
            return Ok(await unitOfWork.Products.GetCollection(x => x.VendorId == vendorId, x => new { 
                x.Id,
                x.Name,
                //x.Hoplink,
                Image = x.Media.Thumbnail
            }));
        }


        // Not sure if this is being used
        //[Route("ProductCount")]
        //[HttpGet]
        //public async Task<ActionResult> GetVendorProductCount(int vendorId)
        //{
        //    return Ok(await unitOfWork.Products.GetCount(x => x.VendorId == vendorId));
        //}



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchVendors(string searchTerm)
        {

            var vendor = await unitOfWork.Vendors.GetCollection(searchTerm, x => new VendorViewModel { Id = x.Id, Name = x.Name, PrimaryEmail = x.PrimaryEmail, PrimaryFirstName = x.PrimaryFirstName, PrimaryLastName = x.PrimaryLastName});

            return Ok(vendor);
        }


        // Not using!!!
        [HttpGet]
        [Route("Duplicate")]
        public async Task<ActionResult> CheckDuplicate(string vendorName)
        {

            var vendor = await unitOfWork.Vendors.Get(x => x.Name.ToUpper() == vendorName.ToUpper());

            return Ok(vendor);
        }
    }
}
