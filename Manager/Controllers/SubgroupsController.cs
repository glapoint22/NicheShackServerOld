using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubgroupsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public SubgroupsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetSubgroups()
        {
            return Ok(await unitOfWork.Subgroups.GetCollection<ItemViewModel<Subgroup>>());
        }




        [HttpPost]
        public async Task<ActionResult> AddSubgroup(ItemViewModel subgroup)
        {
            Subgroup newSubgroup = new Subgroup
            {
                Name = subgroup.Name
            };


            // Add and save
            unitOfWork.Subgroups.Add(newSubgroup);
            await unitOfWork.Save();

            return Ok(newSubgroup.Id);
        }





        [HttpPut]
        public async Task<ActionResult> UpdateSubgroup(ItemViewModel updatedSubgroup)
        {
            Subgroup subgroup = await unitOfWork.Subgroups.Get(updatedSubgroup.Id);

            subgroup.Name = updatedSubgroup.Name;

            // Update and save
            unitOfWork.Subgroups.Update(subgroup);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpDelete]
        public async Task<ActionResult> DeleteSubgroups([FromQuery] int[] ids)
        {
            foreach (int id in ids)
            {
                Subgroup subgroup = await unitOfWork.Subgroups.Get(id);
                unitOfWork.Subgroups.Remove(subgroup);
            }


            await unitOfWork.Save();

            return Ok();
        }


        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchSubgroups(string searchWords)
        {
            return Ok(await unitOfWork.Subgroups.GetCollection<ItemViewModel<Subgroup>>(searchWords));
        }
    }
}
