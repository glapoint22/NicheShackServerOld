using System.Threading.Tasks;
using Manager.Repositories;
using DataAccess.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models;
using Manager.Classes;
using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq;
using Manager.ViewModels;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NichesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public NichesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult> GetSubniches(int parentId)
        {
            //return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(x => x.CategoryId == parentId));


            if (parentId > 0)
            {
                return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(x => x.CategoryId == parentId));
            }
            else
            {
                return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>());
            }
        }



        
        [Route("All")]
        [HttpGet]
        public async Task<ActionResult> GetAllSubniches()
        {
            return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>());
        }



        [Route("Parent")]
        [HttpGet]
        public async Task<ActionResult> GetSubnicheParent(int childId)
        {
            var parentCategory = await unitOfWork.Niches.Get(x => x.Id == childId, x => x.Category);
            return Ok(new {id = parentCategory.Id, name = parentCategory.Name});
        }





        [HttpPut]
        public async Task<ActionResult> UpdateSubnicheName(ItemViewModel niche)
        {
            Niche updatedNiche = await unitOfWork.Niches.Get(niche.Id);

            updatedNiche.Name = niche.Name;
            updatedNiche.UrlName = Utility.GetUrlName(niche.Name);

            // Update and save
            unitOfWork.Niches.Update(updatedNiche);
            await unitOfWork.Save();

            return Ok();
        }





        [HttpPost]
        public async Task<ActionResult> AddSubniche(ItemViewModel niche)
        {
            Niche newNiche = new Niche
            {
                CategoryId = niche.Id,
                Name = niche.Name,
                UrlId = Utility.GetUrlId(),
                UrlName = Utility.GetUrlName(niche.Name)
            };

            unitOfWork.Niches.Add(newNiche);
            await unitOfWork.Save();

            return Ok(newNiche.Id);
        }



        [Route("Move")]
        [HttpPut]
        public async Task<ActionResult> MoveSubniche(MoveItemViewModel moveItem)
        {
            Niche nicheToBeMoved = await unitOfWork.Niches.Get(moveItem.ItemToBeMovedId);

            nicheToBeMoved.CategoryId = moveItem.DestinationItemId;

            // Update and save
            unitOfWork.Niches.Update(nicheToBeMoved);
            await unitOfWork.Save();


            return Ok();
        }





        [HttpDelete]
        public async Task<ActionResult> DeleteSubniche(int id)
        {
            Niche niche = await unitOfWork.Niches.Get(id);

            unitOfWork.Niches.Remove(niche);
            await unitOfWork.Save();

            return Ok();
        }





        [Route("LeadPageIds")]
        [HttpGet]
        public async Task<ActionResult> GetLeadPageIds(int nicheId)
        {
            return Ok(await unitOfWork.LeadPages.GetCollection(x => x.NicheId == nicheId, x => x.Id));
        }


        [Route("LeadPage")]
        [HttpGet]
        public async Task<ActionResult> GetLeadPage(int leadPageId)
        {
            return Ok(await unitOfWork.LeadPages.Get(x => x.Id == leadPageId, x => x.Content));
        }




        [Route("LeadPage/Add")]
        [HttpGet]
        public async Task<ActionResult> AddLeadPage(int nicheId)
        {
            string leadPageName = "New Lead Page";
            string leadPageEmailName = "New Lead Page Email";


            // Create the new lead page
            LeadPage leadPage = new LeadPage
            {
                NicheId = nicheId,
                Name = leadPageName,
                Content = ""
            };

            // Add and save
            unitOfWork.LeadPages.Add(leadPage);
            await unitOfWork.Save();

            // Update the content with the new Id and update
            leadPage.Content = "{\"id\":" + leadPage.Id + ",\"name\":\"" + leadPageName + "\",\"background\":{\"color\":\"#ffffff\"}}";
            unitOfWork.LeadPages.Update(leadPage);




            // Create the new lead page email
            LeadPageEmail leadPageEmail = new LeadPageEmail
            {
                LeadPageId = leadPage.Id,
                Name = leadPageEmailName,
                Content = ""
            };


            // Add and save
            unitOfWork.LeadPageEmails.Add(leadPageEmail);
            await unitOfWork.Save();


            // Update the content with the new Id
            leadPageEmail.Content = "{\"id\":" + leadPageEmail.Id + ",\"name\":\"" + leadPageEmailName + "\",\"background\":{\"color\":\"#ffffff\"}}";


            // Update and save
            unitOfWork.LeadPageEmails.Update(leadPageEmail);
            await unitOfWork.Save();


            // Return the new lead page content
            return Ok(leadPage.Content);
        }




        [Route("LeadPage/Duplicate")]
        [HttpGet]
        public async Task<ActionResult> DuplicateLeadPage(int leadPageId)
        {
            // Get the lead page
            LeadPage leadPage = await unitOfWork.LeadPages.Get(leadPageId);
            leadPage.Id = 0;

            // Add the duplicated lead page and save
            unitOfWork.LeadPages.Add(leadPage);
            await unitOfWork.Save();


            // Update the content with the new id
            leadPage.Content = Regex.Replace(leadPage.Content, "^{\"id\":" + leadPageId, "{\"id\":" + leadPage.Id);
            unitOfWork.LeadPages.Update(leadPage);


            // Get the lead page email
            LeadPageEmail leadPageEmail = await unitOfWork.LeadPageEmails.Get(x => x.LeadPageId == leadPageId);

            // Get the current id for later use
            int leadPageEmailId = leadPageEmail.Id;

            // Reset the id and assign the new lead page id
            leadPageEmail.Id = 0;
            leadPageEmail.LeadPageId = leadPage.Id;


            // Add the new lead page email and save
            unitOfWork.LeadPageEmails.Add(leadPageEmail);
            await unitOfWork.Save();


            // Update the content and save
            leadPageEmail.Content = Regex.Replace(leadPageEmail.Content, "^{\"id\":" + leadPageEmailId, "{\"id\":" + leadPageEmail.Id);
            unitOfWork.LeadPageEmails.Update(leadPageEmail);
            await unitOfWork.Save();


            // Return the lead page content
            return Ok(leadPage.Content);
        }







        //[Route("LeadPage")]
        //[HttpPut]
        //public async Task<ActionResult> UpdateLeadPage(UpdatedPage updatedPage)
        //{
        //    LeadPage leadPage = await unitOfWork.LeadPages.Get(updatedPage.Id);

        //    leadPage.Name = updatedPage.Name;
        //    leadPage.Content = updatedPage.Content;

        //    // Update and save
        //    unitOfWork.LeadPages.Update(leadPage);
        //    await unitOfWork.Save();

        //    return Ok();
        //}





        [Route("LeadPageEmail")]
        [HttpGet]
        public async Task<ActionResult> GetLeadpageEmails(int leadPageId)
        {
            return Ok(await unitOfWork.LeadPageEmails.Get(x => x.LeadPageId == leadPageId, x => x.Content));
        }







        //[Route("LeadPageEmail")]
        //[HttpPut]
        //public async Task<ActionResult> UpdateLeadPageEmail(UpdatedPage updatedPage)
        //{
        //    LeadPageEmail leadPageEmail = await unitOfWork.LeadPageEmails.Get(updatedPage.Id);

        //    leadPageEmail.Name = updatedPage.Name;
        //    leadPageEmail.Content = updatedPage.Content;

        //    // Update and save
        //    unitOfWork.LeadPageEmails.Update(leadPageEmail);
        //    await unitOfWork.Save();

        //    return Ok();
        //}





        [HttpGet]
        [Route("Detail")]
        public async Task<ActionResult> GetNichessDetail()
        {
            return Ok(await unitOfWork.Niches.GetCollection(x => new
            {
                id = x.Id,
                name = x.Name,
                urlName = x.UrlName,
                //icon = new
                //{
                //    name = x.Media.Name,
                //    url = x.Media.ImageAnySize
                //}
            }));
        }





        [HttpGet]
        [Route("Detail/Search")]
        public async Task<ActionResult> DetailSearch(string searchWords)
        {
            return Ok(await unitOfWork.Niches.GetCollection(searchWords, x => new
            {
                id = x.Id,
                name = x.Name,
                urlName = x.UrlName,
                //icon = new
                //{
                //    name = x.Media.Name,
                //    url = x.Media.ImageAnySize
                //}
            }));
        }








        [Route("LeadPage")]
        [HttpDelete]
        public async Task<ActionResult> DeleteLeadPage(int leadPageId)
        {
            LeadPage leadPage = await unitOfWork.LeadPages.Get(leadPageId);

            unitOfWork.LeadPages.Remove(leadPage);
            await unitOfWork.Save();


            return Ok();
        }




        //[HttpPut]
        //[Route("Image")]
        //public async Task<ActionResult> UpdateNicheImage(UpdatedProperty updatedProperty)
        //{
        //    Niche niche = await unitOfWork.Niches.Get(updatedProperty.ItemId);

        //    niche.ImageId = updatedProperty.PropertyId;

        //    // Update and save
        //    unitOfWork.Niches.Update(niche);
        //    await unitOfWork.Save();

        //    return Ok();
        //}




        //[HttpGet]
        //[Route("Image")]
        //public async Task<ActionResult> GetNicheImage(int nicheId)
        //{
        //    return Ok(await unitOfWork.Media.Get(x => x.Id == x.Niches.Where(y => y.Id == nicheId).Select(y => y.ImageId).FirstOrDefault(), x => new ImageViewModel
        //    {
        //        Id = x.Id,
        //        Name = x.Name,
        //        Src = x.ImageAnySize
        //    }));
        //}


        // Not using!!! using validation!!
        [Route("CheckDuplicate")]
        [HttpGet]
        public async Task<ActionResult> CheckDuplicateNiche(int childId, string childName)
        {
            var parentCategoryId = await unitOfWork.Niches.Get(x => x.Id == childId, x => x.CategoryId);
            var niche = await unitOfWork.Niches.Get(x => x.Name == childName && x.CategoryId == parentCategoryId);

            return Ok(niche != null ? new { id = childId, name = childName, parentId = parentCategoryId } : null);
        }



        [Route("Children")]
        [HttpGet]
        public async Task<ActionResult> GetSubnicheChildren(int parentId)
        {
            var products = await unitOfWork.Products.GetCollection(x => x.NicheId == parentId, x => new ItemViewModel { Id = x.Id, Name = x.Name });
            return Ok(products);
        }




        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchSubniches(string searchTerm)
        {
            return Ok(await unitOfWork.Niches.GetCollection<ItemViewModel<Niche>>(searchTerm));
        }
    }
}
