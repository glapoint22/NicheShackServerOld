using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public KeywordsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetKeywords()
        {
            return Ok(await unitOfWork.Keywords.GetCollection<ItemViewModel<Keyword>>());
        }
    }
}
