using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailableKeywordsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public AvailableKeywordsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("Groups")]
        public async Task<ActionResult> GetKeywordGroups(int productId)
        {

            if(productId == 0)
            {
                IEnumerable<KeywordGroup> keywordGroups = await unitOfWork.KeywordGroups.GetCollection(x => !x.ForProduct);
                return Ok(keywordGroups);
            }
            else
            {
                IEnumerable<int> keywordGroupIds = await unitOfWork.KeywordGroups_Belonging_To_Product.GetCollection(x => x.ProductId == productId, x => x.KeywordGroupId);

                IEnumerable<KeywordGroup> keywordGroups = await unitOfWork.KeywordGroups.GetCollection(x => !x.ForProduct);
                return Ok(keywordGroups
                    .Select(x => new
                    {
                        id = x.Id,
                        name = x.Name,
                        forProduct = keywordGroupIds.Contains(x.Id)
                    })
                    .OrderBy(x => x.name));
            }

            
        }




        [HttpGet]
        public async Task<ActionResult> GetKeywords(int parentId)
        {
            var keywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == parentId, x => new
            {
                id = x.Keyword.Id,
                name = x.Keyword.Name
            });
            return Ok(keywords.OrderBy(x => x.name));
        }



        [Route("Parent")]
        [HttpGet]
        public async Task<ActionResult> GetKeywordParent(int childId)
        {
            var parentId = await unitOfWork.Keywords_In_KeywordGroup.Get(x => x.KeywordId == childId, x => x.KeywordGroupId);
            var parent = await unitOfWork.KeywordGroups.Get(x => x.Id == parentId);
            return Ok(new { id = parentId, name = parent.Name });
        }



        [HttpPut]
        [Route("Groups")]
        public async Task<ActionResult> UpdateKeywordGroup(ItemViewModel updatedProperty)
        {
            KeywordGroup keywordGroup = await unitOfWork.KeywordGroups.Get(updatedProperty.Id);

            keywordGroup.Name = updatedProperty.Name;

            // Update and save
            unitOfWork.KeywordGroups.Update(keywordGroup);
            await unitOfWork.Save();

            return Ok();
        }






        [HttpPost]
        [Route("Groups")]
        public async Task<ActionResult> AddKeywordGroup(ItemViewModel keywordGroup)
        {


            KeywordGroup newKeywordGroup = new KeywordGroup
            {
                Name = keywordGroup.Name
            };


            // Add and save
            unitOfWork.KeywordGroups.Add(newKeywordGroup);
            await unitOfWork.Save();

            return Ok(newKeywordGroup.Id);
        }







        [HttpPost]
        public async Task<ActionResult> AddKeyword(ItemViewModel item)
        {
            Keyword newKeyword = new Keyword
            {
                Name = item.Name.Trim().ToLower()
            };


            int keywordId = await unitOfWork.Keywords.Get(x => x.Name == newKeyword.Name, x => x.Id);




            // Add and save
            if (keywordId == 0)
            {
                unitOfWork.Keywords.Add(newKeyword);
                await unitOfWork.Save();
            }
            else
            {
                newKeyword.Id = keywordId;
            }



            unitOfWork.Keywords_In_KeywordGroup.Add(new Keyword_In_KeywordGroup
            {
                KeywordGroupId = item.Id,
                KeywordId = newKeyword.Id
            });


            IEnumerable<int> productIds = await unitOfWork.KeywordGroups_Belonging_To_Product.GetCollection(x => x.KeywordGroupId == item.Id, x => x.ProductId);

            if (productIds.Count() > 0)
            {
                unitOfWork.ProductKeywords.AddRange(productIds.Select(x => new ProductKeyword
                {
                    ProductId = x,
                    KeywordId = newKeyword.Id
                }));
            }


            await unitOfWork.Save();

            return Ok(newKeyword.Id);
        }







        [HttpPut]
        public async Task<ActionResult> UpdateKeyword(ItemViewModel updatedProperty)
        {
            string keywordName = updatedProperty.Name.Trim().ToLower();

            if (await unitOfWork.Keywords.Any(x => x.Name == keywordName)) return Ok();


            Keyword keyword = await unitOfWork.Keywords.Get(updatedProperty.Id);

            keyword.Name = keywordName;

            // Update and save
            unitOfWork.Keywords.Update(keyword);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        public async Task<ActionResult> DeleteKeyword(int id, int keywordGroupId)
        {
            var keywordCount = await unitOfWork.Keywords_In_KeywordGroup.GetCount(x => x.KeywordId == id);

            if(keywordCount > 1)
            {
               var keyword = await unitOfWork.Keywords_In_KeywordGroup.Get(x => x.KeywordId == id && x.KeywordGroupId == keywordGroupId);
                unitOfWork.Keywords_In_KeywordGroup.Remove(keyword);
            }
            else
            {
                var keyword = await unitOfWork.Keywords.Get(id);
                unitOfWork.Keywords.Remove(keyword);
            }





            await unitOfWork.Save();

            return Ok();
        }





        [HttpDelete]
        [Route("Groups")]
        public async Task<ActionResult> DeleteKeywordGroup(int id)
        {
            KeywordGroup keywordGroup = await unitOfWork.KeywordGroups.Get(id);

            IEnumerable<Keyword> keywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == id, x => x.Keyword);

            unitOfWork.Keywords.RemoveRange(keywords);

            unitOfWork.KeywordGroups.Remove(keywordGroup);
            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        [Route("Remove")]
        public async Task<ActionResult> RemoveKeyword([FromQuery] int[] ids)
        {
            Keyword_In_KeywordGroup keyword_In_KeywordGroup = await unitOfWork.Keywords_In_KeywordGroup.Get(x => x.KeywordGroupId == ids[0] && x.KeywordId == ids[1]);

            unitOfWork.Keywords_In_KeywordGroup.Remove(keyword_In_KeywordGroup);
            await unitOfWork.Save();

            return Ok();
        }



        [HttpGet]
        [Route("Groups/Search")]
        public async Task<ActionResult> SearchKeywordGroup(int productId, string searchWords)
        {
            var keywordGroupIds = await unitOfWork.KeywordGroups_Belonging_To_Product.GetCollection(x => x.ProductId == productId, x => x.KeywordGroupId);
            var keywordGroups = await unitOfWork.KeywordGroups.GetCollection(x => !x.ForProduct, searchWords, x => new KeywordSearchItem { Id = x.Id, Name = x.Name, Type = "Group", ForProduct = keywordGroupIds.Contains(x.Id) });
            var customKeywordIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == x.KeywordGroup.Id && x.KeywordGroup.ForProduct, x => x.KeywordId);
            var keywords = await unitOfWork.Keywords.GetCollection(x => !customKeywordIds.Contains(x.Id), searchWords, x => new KeywordSearchItem { Id = x.Id, Name = x.Name, Type = "Keyword", ForProduct = x.Keywords_In_KeywordGroup.Any(y => y.KeywordId == x.Id && keywordGroupIds.Contains(y.KeywordGroupId)) });
            var searchResults = keywordGroups.Concat(keywords).OrderBy(x => x.Name).ToList();
            return Ok(searchResults);
        }

            

        [Route("CheckDuplicate")]
        [HttpGet]
        public async Task<ActionResult> CheckDuplicateKeyword(int childId, string childName)
        {
            var keywordGroupId = await unitOfWork.Keywords_In_KeywordGroup.Get(x => x.KeywordId == childId, x => x.KeywordGroupId);
            var keywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == keywordGroupId, x => x.Keyword.Name);
            var duplicateFound = keywords.Any(x => x.ToUpper() == childName.ToUpper());


            return Ok(!duplicateFound ? null : new { id = childId, name = childName, parentId = keywordGroupId });
        }
    }
}
