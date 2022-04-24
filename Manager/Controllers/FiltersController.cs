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




        [HttpPost]
        public async Task<ActionResult> AddFilter(ItemViewModel filter)
        {
            Filter newFilter = new Filter
            {
                Name = filter.Name
            };


            unitOfWork.Filters.Add(newFilter);
            await unitOfWork.Save();

            return Ok(newFilter.Id);
        }





        [HttpDelete]
        public async Task<ActionResult> DeleteFilter(int id)
        {
            Filter filter = await unitOfWork.Filters.Get(id);

            unitOfWork.Filters.Remove(filter);
            await unitOfWork.Save();

            return Ok();
        }






        [HttpPost]
        [Route("Options")]
        public async Task<ActionResult> AddFilterOption(ItemViewModel filterOption)
        {
            FilterOption newFilterOption = new FilterOption
            {
                FilterId = filterOption.Id,
                Name = filterOption.Name
            };


            unitOfWork.FilterOptions.Add(newFilterOption);
            await unitOfWork.Save();

            return Ok(newFilterOption.Id);
        }






        [HttpDelete]
        [Route("Options")]
        public async Task<ActionResult> DeleteFilterOption(int id)
        {
            FilterOption filterOption = await unitOfWork.FilterOptions.Get(id);

            unitOfWork.FilterOptions.Remove(filterOption);
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


        [Route("CheckDuplicate")]
        [HttpGet]
        public async Task<ActionResult> CheckDuplicateFilterOption(int filterOptionId, string filterOptionName)
        {
            var parentFilterId = await unitOfWork.FilterOptions.Get(x => x.Id == filterOptionId, x => x.Filter.Id);
            var filterOption = await unitOfWork.FilterOptions.Get(x => x.Name == filterOptionName && x.Filter.Id == parentFilterId);

            return Ok(filterOption);
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
