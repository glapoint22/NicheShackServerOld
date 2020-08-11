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
    public class FiltersController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public FiltersController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        [HttpGet]
        public async Task<ActionResult> GetFilters()
        {
            return Ok(await unitOfWork.Filters.GetCollection<ItemViewModel<Filter>>());
        }




        [HttpPut]
        public async Task<ActionResult> UpdateFilterName(ItemViewModel updatedFilter)
        {
            Filter filter = await unitOfWork.Filters.Get(updatedFilter.Id);

            filter.Name = updatedFilter.Name;

            // Update and save
            unitOfWork.Filters.Update(filter);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpPut]
        [Route("Options")]
        public async Task<ActionResult> UpdateFilteOptionrName(ItemViewModel updatedFilterOption)
        {
            FilterOption filterOption = await unitOfWork.FilterOptions.Get(updatedFilterOption.Id);

            filterOption.Name = updatedFilterOption.Name;

            // Update and save
            unitOfWork.FilterOptions.Update(filterOption);
            await unitOfWork.Save();

            return Ok();
        }






        [Route("Options")]
        [HttpGet]
        public async Task<ActionResult> GetOptions(int filterId)
        {
            return Ok(await unitOfWork.FilterOptions.GetCollection<ItemViewModel<FilterOption>>(x => x.FilterId == filterId));
        }



        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchFilters(string searchWords)
        {
            return Ok(await unitOfWork.Filters.GetCollection<ItemViewModel<Filter>>(searchWords));
        }



        [HttpGet]
        [Route("Options/Search")]
        public async Task<ActionResult> SearchFilterOptions(string searchWords)
        {
            return Ok(await unitOfWork.FilterOptions.GetCollection<ItemViewModel<FilterOption>>(searchWords));
        }
    }
}
