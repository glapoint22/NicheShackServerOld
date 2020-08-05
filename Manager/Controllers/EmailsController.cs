using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.ViewModels;
using Manager.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public EmailsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<ActionResult> GetEmails()
        {
            return Ok(await unitOfWork.Emails.GetCollection(x => new { 
                x.Id,
                x.Type
            }));
        }



        [HttpGet]
        [Route("Email")]
        public async Task<ActionResult> GetEmail(int emailId)
        {
            return Ok(await unitOfWork.Emails.Get(x => x.Id == emailId, x => x.Content));
        }
    }
}
