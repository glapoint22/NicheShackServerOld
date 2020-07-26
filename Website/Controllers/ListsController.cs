using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using DataAccess.Models;
using Website.Repositories;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public ListsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }



        // ..................................................................................List Exists......................................................................
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        [Route("ListExists")]
        public async Task<ActionResult> ListExists(string listId)
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return Ok(await unitOfWork.Collaborators.Any(x => x.ListId == listId && x.CustomerId == customerId));
        }



        // ..................................................................................Get List Owner......................................................................
        [HttpGet]
        [Route("ListOwner")]
        public async Task<ActionResult> GetListOwner(string listId)
        {
            return Ok(await unitOfWork.Collaborators.Get(x => x.ListId == listId && x.IsOwner, x => x.Customer.FirstName));
        }



        // ..................................................................................Get Lists......................................................................
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetLists()
        {
            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            
            return Ok(await unitOfWork.Lists.GetLists(customerId));
        }





        // ..................................................................................Get Sort Options......................................................................
        [HttpGet]
        [Route("SortOptions")]
        public ActionResult GetSortOptions()
        {
            return Ok(new ListProductDTO().GetSortOptions());
        }






        // ..................................................................................Get List Data......................................................................
        [HttpGet]
        [Route("ListData")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetListData(string listId)
        {
            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Get all collaborators from the selected list
            IEnumerable<Collaborator> collaborators = await unitOfWork.Collaborators
                    .GetCollection(x => x.ListId == listId, x => new Collaborator
                    {
                        CustomerId = x.CustomerId,
                        ListId = x.ListId,
                        IsOwner = x.IsOwner,
                        Name = x.Customer.FirstName
                    });

            // Is the customer the owner of this list?
            bool isOwner = collaborators.Any(x => x.CustomerId == customerId && x.ListId == listId && x.IsOwner);

            return Ok(new
            {
                // If the customer is the owner of this list, get collaborators (excluding the owner)
                collaborators = isOwner ? collaborators.Where(x => !x.IsOwner).Select(x => new
                {
                    customerId = x.CustomerId,
                    name = x.Name
                }).ToList() : null,
                isOwner,
                ownerName = collaborators.Where(x => x.IsOwner).Select(x => x.Name).SingleOrDefault()
            });
        }







        // ..................................................................................Get List Products......................................................................
        [HttpGet]
        [Route("Products")]
        public async Task<ActionResult> GetListProducts(string listId, string sort = "")
        {
            // Get the customer Id
            string customerId = await unitOfWork.Collaborators.Get(x => x.ListId == listId && x.IsOwner, x => x.CustomerId);

            // Get all collaborator ids from this list
            IEnumerable<Guid> collaboratorIds = await unitOfWork.Collaborators
                    .GetCollection(x => x.ListId == listId, x => x.Id);

            return Ok(await unitOfWork.Lists.GetListProducts(collaboratorIds, customerId, sort));
        }







        // ..................................................................................Create List....................................................................
        [HttpPost]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> CreateList(List list)
        {
            // Make sure the list name is not empty
            if (list.Name == null) return BadRequest(ModelState);


            // Get the customer id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;



            // Create the new list and add it to the database
            List newList = new List
            {
                Id = Guid.NewGuid().ToString("N").ToUpper(),
                Name = list.Name,
                Description = list.Description,
                CollaborateId = Guid.NewGuid().ToString("N").ToUpper()
            };

            unitOfWork.Lists.Add(newList);


            // Set the owner as the first collaborator of the list
            ListCollaborator collaborator = new ListCollaborator
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ListId = newList.Id,
                IsOwner = true
            };

            unitOfWork.Collaborators.Add(collaborator);


            // Save all updates to the database
            await unitOfWork.Save();


            // Return the new list id to the client
            return Ok(new
            {
                listId = newList.Id
            });
        }







        // ..................................................................................Update List....................................................................
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdateList(List list)
        {
            // Get the customer id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Make sure this customer is the owner of this list
            if (!await unitOfWork.Collaborators.Any(x => x.ListId == list.Id && x.CustomerId == customerId && x.IsOwner)) return Unauthorized();


            // Get the list from the database
            List updatedList = await unitOfWork.Lists.Get(list.Id);

            // Update the list with the new data
            if (updatedList != null)
            {
                updatedList.Name = list.Name;
                updatedList.Description = list.Description;

                // Update and save
                unitOfWork.Lists.Update(updatedList);
                await unitOfWork.Save();

                return Ok(new
                {
                    name = list.Name,
                    description = list.Description
                });
            }

            return BadRequest();
        }







        // ..................................................................................Delete List....................................................................
        [HttpDelete]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> DeleteList(string listId)
        {
            // Get the customer id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Make sure this customer is the owner of this list
            if (!await unitOfWork.Collaborators.Any(x => x.ListId == listId && x.CustomerId == customerId && x.IsOwner))
            {
                return Unauthorized();
            }

            // Get the list from the database
            List list = await unitOfWork.Lists.Get(x => x.Id == listId);

            // If the list is found, delete it
            if (list != null)
            {
                unitOfWork.Lists.Remove(list);
                await unitOfWork.Save();
                return Ok();
            }

            return NotFound();
        }






        // .........................................................................Remove Collaborator.....................................................................
        [Route("Collaborator")]
        [HttpDelete]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> RemoveCollaborator(string customerId, string listId)
        {
            // Make sure we are the owner of the list
            if (!await unitOfWork.Collaborators.Any(x => x.ListId == listId && x.CustomerId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.IsOwner))
            {
                return Unauthorized();
            }

            // Get the collaborator to delete
            ListCollaborator collaborator = await unitOfWork.Collaborators.Get(x => x.CustomerId == customerId && x.ListId == listId);

            // If found, delete the collaborator
            if (collaborator != null)
            {
                unitOfWork.Collaborators.Remove(collaborator);
                await unitOfWork.Save();
                return Ok();
            }

            return NotFound();
        }
    }
}