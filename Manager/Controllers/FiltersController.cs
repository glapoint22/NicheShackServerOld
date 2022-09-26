using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
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



        [Route("Options/Parent")]
        [HttpGet]
        public async Task<ActionResult> GetFilter(int childId)
        {
            var parentId = await unitOfWork.FilterOptions.Get(x => x.Id == childId, x => x.FilterId);
            var parent = await unitOfWork.Filters.Get(x => x.Id == parentId);
            return Ok(new { id = parentId, name = parent.Name });
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







        [HttpGet]
        [Route("Options")]
        public async Task<ActionResult> GetFilterOptions(int parentId, int productId)
        {

            if (productId == 0)
            {
                return Ok(await unitOfWork.FilterOptions.GetCollection<ItemViewModel<FilterOption>>(x => x.FilterId == parentId));
            }
            else
            {
                return Ok(await unitOfWork.Products.GetProductFilters(productId, parentId));
            }
        }






        // Not using!!! using validation!!
        [Route("Options/CheckDuplicate")]
        [HttpGet]
        public async Task<ActionResult> CheckDuplicateFilterOption(int childId, string childName)
        {
            var parentFilterId = await unitOfWork.FilterOptions.Get(x => x.Id == childId, x => x.Filter.Id);
            var filterOption = await unitOfWork.FilterOptions.Get(x => x.Name == childName && x.Filter.Id == parentFilterId);

            return Ok(filterOption != null ? new { id = childId, name = childName, parentId = parentFilterId } : null);
        }





        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchFilters(int productId, string searchWords)
        {
            var filters = await unitOfWork.Filters.GetCollection(searchWords, x => new CheckboxSearchItem { Id = x.Id, Name = x.Name, Type = "Filter" });
            var filterOptions = await unitOfWork.FilterOptions.GetCollection(searchWords, x => new CheckboxSearchItem { Id = x.Id, Name = x.Name, Type = "Option", Checked = x.ProductFilters.Select(y => y.ProductId).Contains(productId) });
            var searchResults = filters.Concat(filterOptions).OrderBy(x => x.Name).ToList();
            return Ok(searchResults);
        }
    }
}
