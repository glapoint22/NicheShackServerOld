using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult> GetSubgroups(int productId)
        {
            if (productId == 0)
            {
                IEnumerable<ItemViewModel<Subgroup>> subgroups = await unitOfWork.Subgroups.GetCollection<ItemViewModel<Subgroup>>();
                return Ok(subgroups.OrderBy(x => x.Name));
            }
            else
            {
                var subgroups = await unitOfWork.Subgroups.GetCollection(x => new { Id = x.Id, Name = x.Name, Checked = x.SubgroupProducts.Where(y => y.ProductId == productId).Select(y => y.SubgroupId).Contains(x.Id) });
                return Ok(subgroups.OrderBy(x => x.Name));
            }
        }




        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchSubgroups(int productId, string searchWords)
        {
            var subgroups = await unitOfWork.Subgroups.GetCollection(searchWords, x => new { Id = x.Id, Name = x.Name, Checked = x.SubgroupProducts.Where(y => y.ProductId == productId).Select(y => y.SubgroupId).Contains(x.Id) });

            return Ok(subgroups.OrderBy(x => x.Name));
        }




        [HttpPost]
        public async Task<ActionResult> AddSubgroup(ItemViewModel subgroup)
        {
            string subgroupName = subgroup.Name.Trim();

            if (await unitOfWork.Subgroups.Any(x => x.Name.ToLower() == subgroupName.ToLower())) return Ok();


            Subgroup newSubgroup = new Subgroup
            {
                Name = subgroupName
            };


            // Add and save
            unitOfWork.Subgroups.Add(newSubgroup);
            await unitOfWork.Save();

            return Ok(newSubgroup.Id);
        }










        [HttpPut]
        public async Task<ActionResult> UpdateSubgroup(ItemViewModel updatedSubgroup)
        {
            string subgroupName = updatedSubgroup.Name.Trim();

            if (await unitOfWork.Subgroups.Any(x => x.Name.ToLower() == subgroupName.ToLower())) return Ok();

            Subgroup subgroup = await unitOfWork.Subgroups.Get(updatedSubgroup.Id);

            subgroup.Name = subgroupName;

            // Update and save
            unitOfWork.Subgroups.Update(subgroup);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        public async Task<ActionResult> DeleteSubgroup(int id)
        {
            Subgroup subgroup = await unitOfWork.Subgroups.Get(id);

            unitOfWork.Subgroups.Remove(subgroup);
            await unitOfWork.Save();

            return Ok();
        }



        //[HttpDelete]
        //public async Task<ActionResult> DeleteSubgroups([FromQuery] int[] ids)
        //{
        //    foreach (int id in ids)
        //    {
        //        Subgroup subgroup = await unitOfWork.Subgroups.Get(id);
        //        unitOfWork.Subgroups.Remove(subgroup);
        //    }


        //    await unitOfWork.Save();

        //    return Ok();
        //}
    }
}
