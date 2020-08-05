using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public MediaController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<ActionResult> GetProducts(int type)
        {
            return Ok(await unitOfWork.Media.GetCollection(x => x.Type == type, x => new  { 
                x.Id,
                x.Name,
                x.Url,
                x.Thumbnail
            }));
        }
    }
}
