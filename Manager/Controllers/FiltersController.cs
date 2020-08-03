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


        [Route("Options")]
        [HttpGet]
        public async Task<ActionResult> GetOptions(int filterId)
        {
            return Ok(await unitOfWork.FilterOptions.GetCollection<ItemViewModel<FilterOption>>(x => x.FilterId == filterId));
        }
    }
}
