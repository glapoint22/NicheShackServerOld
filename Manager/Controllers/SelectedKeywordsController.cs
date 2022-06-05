using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
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
    public class SelectedKeywordsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public SelectedKeywordsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("Groups")]
        public async Task<ActionResult> GetKeywordGroups(int productId)
        {
            IEnumerable<int> keywordGroupIds = await unitOfWork.KeywordGroups_Belonging_To_Product.GetCollection(x => x.ProductId == productId, x => x.KeywordGroupId);

            IEnumerable<KeywordGroup> keywordGroups = await unitOfWork.KeywordGroups.GetCollection(x => keywordGroupIds.Contains(x.Id));

            return Ok(keywordGroups.Select(x => new
            {
                id = x.Id,
                name = x.Name,
                forProduct = x.ForProduct
            })
                .OrderBy(x => x.name));
        }



        [HttpGet]
        public async Task<ActionResult> GetKeywords(int productId, int parentId)
        {
            var keywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == parentId, x => new
            {
                id = x.Keyword.Id,
                name = x.Keyword.Name,
                Checked = x.Keyword.ProductKeywords.Any(z => z.ProductId == productId && z.KeywordId == x.Keyword.Id),
                forProduct = x.KeywordGroup.ForProduct

            });
            return Ok(keywords.OrderBy(x => x.name));
        }





        [HttpPut]
        [Route("Update")]
        public async Task<ActionResult> UpdateKeyword(UpdatedProductItem updatedProductItem)
        {

            if (updatedProductItem.Checked)
            {
                unitOfWork.ProductKeywords.Add(
                    new ProductKeyword
                    {
                        KeywordId = updatedProductItem.Id,
                        ProductId = updatedProductItem.ProductId
                    });
            }
            else
            {
                ProductKeyword productKeyword = await unitOfWork.ProductKeywords.Get(x => x.ProductId == updatedProductItem.ProductId && x.KeywordId == updatedProductItem.Id);
                unitOfWork.ProductKeywords.Remove(productKeyword);
            }


            await unitOfWork.Save();

            return Ok();
        }




        [HttpPost]
        [Route("Groups/Add")]
        public async Task<ActionResult> AddKeywordGroup(UpdatedProductItem updatedProductItem)
        {
            unitOfWork.KeywordGroups_Belonging_To_Product.Add(new KeywordGroup_Belonging_To_Product
            {
                ProductId = updatedProductItem.ProductId,
                KeywordGroupId = updatedProductItem.Id
            });

            IEnumerable<int> keywordIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == updatedProductItem.Id, x => x.KeywordId);

            IEnumerable<ProductKeyword> productKeywords = keywordIds.Select(x => new ProductKeyword
            {
                ProductId = updatedProductItem.ProductId,
                KeywordId = x
            }).ToList();

            unitOfWork.ProductKeywords.AddRange(productKeywords);

            await unitOfWork.Save();

            return Ok();
        }



        [HttpPost]
        [Route("Groups/AddKeyword")]
        public async Task<ActionResult> AddKeywordFromKeywordGroup(NewProductKeyword newProductKeyword)
        {
            unitOfWork.KeywordGroups_Belonging_To_Product.Add(new KeywordGroup_Belonging_To_Product
            {
                ProductId = newProductKeyword.ProductId,
                KeywordGroupId = newProductKeyword.KeywordGroupId
            });


            ProductKeyword productKeyword = new ProductKeyword
            {
                ProductId = newProductKeyword.ProductId,
                KeywordId = newProductKeyword.KeywordId
            };

            unitOfWork.ProductKeywords.Add(productKeyword);

            await unitOfWork.Save();

            return Ok();
        }



        [HttpPost]
        [Route("Groups")]
        public async Task<ActionResult> NewKeywordGroup(ItemViewModel item)
        {
            KeywordGroup keywordGroup = new KeywordGroup
            {
                Name = item.Name,
                ForProduct = true
            };

            unitOfWork.KeywordGroups.Add(keywordGroup);

            await unitOfWork.Save();

            unitOfWork.KeywordGroups_Belonging_To_Product.Add(new KeywordGroup_Belonging_To_Product
            {
                ProductId = item.Id,
                KeywordGroupId = keywordGroup.Id
            });

            await unitOfWork.Save();

            return Ok(keywordGroup.Id);
        }





        [HttpPut]
        [Route("Groups/Remove")]
        public async Task<ActionResult> RemoveKeywordGroup(UpdatedProductItem updatedProductItem)
        {
            IEnumerable<int> KeywordIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == updatedProductItem.Id, x => x.KeywordId);

            IEnumerable<ProductKeyword> productKeywords = await unitOfWork.ProductKeywords.GetCollection(x => KeywordIds.Contains(x.KeywordId) && x.ProductId == updatedProductItem.ProductId);


            unitOfWork.ProductKeywords.RemoveRange(productKeywords);


            unitOfWork.KeywordGroups_Belonging_To_Product.Remove(new KeywordGroup_Belonging_To_Product
            {
                ProductId = updatedProductItem.ProductId,
                KeywordGroupId = updatedProductItem.Id
            });

            await unitOfWork.Save();

            return Ok();
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
        [Route("Groups")]
        public async Task<ActionResult> DeleteKeywordGroup(int id)
        {
            IEnumerable<int> KeywordIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == id, x => x.KeywordId);


            IEnumerable<Keyword> keywords = await unitOfWork.Keywords.GetCollection(x => KeywordIds.Contains(x.Id));

            unitOfWork.Keywords.RemoveRange(keywords);


            KeywordGroup keywordGroup = await unitOfWork.KeywordGroups.Get(id);

            unitOfWork.KeywordGroups.Remove(keywordGroup);

            await unitOfWork.Save();

            return Ok();
        }




        [HttpDelete]
        public async Task<ActionResult> DeleteKeyword(int id)
        {
            Keyword keyword = await unitOfWork.Keywords.Get(id);

            unitOfWork.Keywords.Remove(keyword);

            await unitOfWork.Save();

            return Ok();
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


            int productId = await unitOfWork.KeywordGroups_Belonging_To_Product.Get(x => x.KeywordGroupId == item.Id, x => x.ProductId);

            unitOfWork.ProductKeywords.Add(new ProductKeyword
            {
                ProductId = productId,
                KeywordId = newKeyword.Id
            });

            await unitOfWork.Save();

            return Ok(newKeyword.Id);
        }



        [HttpGet]
        [Route("Groups/Search")]
        public async Task<ActionResult> SearchKeywordGroup(int productId, string searchWords)
        {
            IEnumerable<int> keywordGroupIds = await unitOfWork.KeywordGroups_Belonging_To_Product.GetCollection(x => x.ProductId == productId, x => x.KeywordGroupId);
            IEnumerable<int> keywordIds = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => keywordGroupIds.Contains(x.KeywordGroupId), x => x.Keyword.Id);
            IEnumerable<KeywordSearchItem> keywordGroups = await unitOfWork.KeywordGroups.GetCollection(x => keywordGroupIds.Contains(x.Id), searchWords, x => new KeywordSearchItem { Id = x.Id, Name = x.Name, Type = "Group", ForProduct = x.ForProduct });
            IEnumerable<KeywordSearchItem> keywords = await unitOfWork.Keywords.GetCollection(x => keywordIds.Contains(x.Id), searchWords, x => new KeywordSearchItem { Id = x.Id, Name = x.Name, Type = "Keyword", ForProduct = x.Keywords_In_KeywordGroup.Select(y => y.KeywordGroup.ForProduct).FirstOrDefault(), Checked = x.ProductKeywords.Any(y => y.ProductId == productId && y.KeywordId == x.Id) });
            List<KeywordSearchItem> searchResults = keywordGroups.Concat(keywords).OrderBy(x => x.Name).ToList();

            return Ok(searchResults);
        }



        [Route("CheckDuplicate")]
        [HttpGet]
        public async Task<ActionResult> CheckDuplicateKeyword(int childId, string childName)
        {
            var keywordGroupId = await unitOfWork.Keywords_In_KeywordGroup.Get(x => x.KeywordId == childId, x => x.KeywordGroupId);
            var keywords = await unitOfWork.Keywords_In_KeywordGroup.GetCollection(x => x.KeywordGroupId == keywordGroupId, x => x.Keyword.Name);
            var keyword = keywords.Contains(childName);


            return Ok(keyword ? new { id = childId, name = childName, parentId = keywordGroupId } : null);
        }



        [Route("Parent")]
        [HttpGet]
        public async Task<ActionResult> GetKeywordParent(int childId)
        {
            var parentId = await unitOfWork.Keywords_In_KeywordGroup.Get(x => x.KeywordId == childId, x => x.KeywordGroupId);
            var parent = await unitOfWork.KeywordGroups.Get(x => x.Id == parentId, x => new { id = x.Id, name = x.Name });
            return Ok(parent);
        }
    }
}
