﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Classes;
using DataAccess.Models;
using Website.Repositories;
using DataAccess.ViewModels;
using Services;
using Services.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly EmailService emailService;
        private readonly IWebHostEnvironment env;

        public ListsController(IUnitOfWork unitOfWork, EmailService emailService, IWebHostEnvironment env)
        {
            this.unitOfWork = unitOfWork;
            this.emailService = emailService;
            this.env = env;
        }



        // ..................................................................................Get Shared List......................................................................
        [HttpGet]
        [Route("SharedList")]
        public async Task<ActionResult> GetSharedList(string listId, string sort)
        {
            var listName = await unitOfWork.Lists.Get(x => x.Id == listId, x => x.Name);

            if (listName == null) return Ok();

            return Ok(new
            {
                id = listId,
                name = listName,
                products = await unitOfWork.Lists.GetListProducts(await unitOfWork.Collaborators
                    .GetCollection(x => x.ListId == listId, x => x.Id), null, sort)
            });
        }





        // ..................................................................................Get First List Id......................................................................
        [HttpGet]
        [Route("FirstList")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetFirstListId()
        {
            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            return Ok(new
            {
                id = await unitOfWork.Lists.FirstList(customerId)
            });
        }



        // ..................................................................................Get Lists......................................................................
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetLists(string firstListId)
        {
            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            return Ok(await unitOfWork.Lists.GetLists(customerId,  firstListId));
        }





        // ..................................................................................Get Dropdown Lists......................................................................
        [HttpGet]
        [Route("DropdownLists")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetDropdownLists()
        {
            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var lists = await unitOfWork.Collaborators.GetCollection(x => x.CustomerId == customerId && x.AddToList == true, x => new
            {
                key = x.List.Name,
                value = x.ListId
            });


            return Ok(lists);
        }





        // ..................................................................................Get Collaborators......................................................................
        [HttpGet]
        [Route("Collaborators")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetCollaborators(string listId)
        {
            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!await unitOfWork.Collaborators.Any(x => x.ListId == listId && x.CustomerId == customerId)) return Ok();


            // Get all collaborators from the selected list
            IEnumerable<Collaborator> collaborators = await unitOfWork.Collaborators
                    .GetCollection(x => x.ListId == listId && !x.IsOwner && !x.IsRemoved, x => new Collaborator
                    {
                        Id = x.Id,
                        Name = x.Customer.FirstName + " " + x.Customer.LastName,
                        ListPermissions = new ListPermissions
                        {
                            AddToList = x.AddToList,
                            ShareList = x.ShareList,
                            EditList = x.EditList,
                            InviteCollaborators = x.InviteCollaborators,
                            DeleteList = x.DeleteList,
                            MoveItem = x.MoveItem,
                            RemoveItem = x.RemoveItem
                        }
                    });

            return Ok(collaborators);
        }







        // ..................................................................................Get List Products......................................................................
        [HttpGet]
        [Route("Products")]
        public async Task<ActionResult> GetListProducts(string listId, string sort = "")
        {
            string customerId = null;

            // Get the customer Id
            if (User.Claims.Count() > 0) customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Get all collaborator ids from this list
            IEnumerable<int> collaboratorIds = await unitOfWork.Collaborators
                    .GetCollection(x => x.ListId == listId, x => x.Id);

            return Ok(await unitOfWork.Lists.GetListProducts(collaboratorIds, customerId, sort));
        }







        // ..................................................................................Create List....................................................................
        [HttpPost]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> CreateList(NewList list)
        {
            // Make sure the list name is not empty
            if (list.Name == null) return BadRequest(ModelState);


            // Get the customer id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;



            // Create the new list and add it to the database
            List newList = new List
            {
                Id = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                Name = list.Name,
                Description = list.Description,
                CollaborateId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()
            };

            unitOfWork.Lists.Add(newList);


            // Set the owner as the first collaborator of the list
            ListCollaborator collaborator = new ListCollaborator
            {
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
                id = newList.Id,
                collaborateId = newList.CollaborateId
            });
        }









        // ..................................................................................Edit List....................................................................
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> EditList(UpdatedList list)
        {
            // Get the customer id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Make sure this customer can edit this list
            if (!await unitOfWork.Collaborators.Any(x => (x.ListId == list.Id && x.CustomerId == customerId && x.EditList == true) || (x.ListId == list.Id && x.CustomerId == customerId && x.IsOwner)))
            {
                return Unauthorized();
            }


            // Get the list from the database
            List updatedList = await unitOfWork.Lists.Get(list.Id);

            // Update the list with the new data
            if (updatedList != null)
            {
                string previousName = updatedList.Name;

                updatedList.Name = list.Name;
                updatedList.Description = list.Description;

                // Update and save
                unitOfWork.Lists.Update(updatedList);
                await unitOfWork.Save();



                // Setup the email
                if (previousName != list.Name)
                {
                    await SetupChangedListName(new EmailSetupParams
                    {
                        Host = GetHost(),
                        CustomerId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                        ListId1 = updatedList.Id,
                        Var1 = previousName,
                        Var2 = list.Name
                    });


                }


                return Ok(new
                {
                    name = list.Name,
                    description = list.Description
                });
            }

            return BadRequest();
        }









        // .........................................................................Setup Changed List Name.....................................................................
        private async Task SetupChangedListName(EmailSetupParams emailSetupParams)
        {
            List<string> recipientIds = await unitOfWork.Lists.GetRecipientIds(EmailType.ListNameChange, emailSetupParams.ListId1, emailSetupParams.CustomerId);
            EmailParams emailParams = await unitOfWork.Lists.GetEmailParams(emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, recipientIds);

            if (emailParams == null) return;


            // Send email
            await emailService.SendEmail(EmailType.ListNameChange, "List name changed", emailParams.Recipients, new EmailProperties
            {
                Var1 = emailSetupParams.Var1,
                Var2 = emailSetupParams.Var2,
                Person = emailParams.Collaborator
            });
        }








        // ..................................................................................Delete List....................................................................
        [HttpDelete]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> DeleteList(string listId)
        {
            // Get the customer id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Make sure this customer can delete this list
            if (!await unitOfWork.Collaborators.Any(x => (x.ListId == listId && x.CustomerId == customerId && x.DeleteList) || (x.ListId == listId && x.CustomerId == customerId && x.IsOwner)))
            {
                return Unauthorized();
            }

            // Get the list from the database
            List list = await unitOfWork.Lists.Get(x => x.Id == listId);

            // If the list is found, delete it
            if (list != null)
            {
                unitOfWork.Lists.Remove(list);


                IEnumerable<string> customerIds = await unitOfWork.Collaborators.GetCollection(x => x.ListId == list.Id && !x.IsRemoved && x.Customer.EmailPrefDeletedList == true, x => x.CustomerId);

                if (customerIds.Count() > 1)
                {
                    IEnumerable<Recipient> recipients = await unitOfWork.Customers.GetCollection(x => customerIds.Contains(x.Id), x => new Recipient
                    {
                        CustomerId = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email
                    });



                    // Send Email
                    await emailService.SendEmail(EmailType.DeletedList, "List has been deleted", recipients.Where(x => x.CustomerId != customerId).ToList(), new EmailProperties
                    {
                        Person = recipients.Where(x => x.CustomerId == customerId).Select(x => new Person
                        {
                            FirstName = x.FirstName,
                            LastName = x.LastName
                        }).Single(),
                        Var1 = list.Name
                    });
                }


                await unitOfWork.Save();
                return Ok();
            }

            return NotFound();
        }




        // .........................................................................Update Collaborators.....................................................................
        [Route("UpdateCollaborators")]
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdateCollaborators(List<Collaborator> collaborators)
        {
            // Get the list id
            var listId = await unitOfWork.Collaborators.Get(x => x.Id == collaborators[0].Id, x => x.ListId);

            //Make sure we are the owner of the list
            if (!await unitOfWork.Collaborators.Any(x => x.ListId == listId && x.CustomerId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.IsOwner))
            {
                return Unauthorized();
            }


            foreach (var currentCollaborator in collaborators)
            {
                var collaborator = await unitOfWork.Collaborators.Get(currentCollaborator.Id);

                if (currentCollaborator.IsRemoved)
                {
                    collaborator.IsRemoved = true;

                    // Setup the email
                    await SetupRemovedCollaborator(new EmailSetupParams
                    {
                        CollaboratorId = collaborator.Id,
                        ListId1 = listId
                    });
                }
                else
                {
                    collaborator.AddToList = currentCollaborator.ListPermissions.AddToList;
                    collaborator.ShareList = currentCollaborator.ListPermissions.ShareList;
                    collaborator.EditList = currentCollaborator.ListPermissions.EditList;
                    collaborator.InviteCollaborators = currentCollaborator.ListPermissions.InviteCollaborators;
                    collaborator.DeleteList = currentCollaborator.ListPermissions.DeleteList;
                    collaborator.MoveItem = currentCollaborator.ListPermissions.MoveItem;
                    collaborator.RemoveItem = currentCollaborator.ListPermissions.RemoveItem;
                }

                unitOfWork.Collaborators.Update(collaborator);
            };

            await unitOfWork.Save();

            return Ok();
        }











        // .........................................................................Setup Removed Collaborator.....................................................................
        private async Task SetupRemovedCollaborator(EmailSetupParams emailSetupParams)
        {
            // Get the customer id
            string customerId = await unitOfWork.Collaborators.Get(x => x.Id == emailSetupParams.CollaboratorId && x.Customer.EmailPrefRemovedCollaborator == true, x => x.CustomerId);

            if (customerId == null) return;


            // Get the recipient
            var recipient = await unitOfWork.Customers.Get(x => x.Id == customerId, x => new
            {
                firstName = x.FirstName,
                lastName = x.LastName,
                email = x.Email
            });



            // Get the list
            string list = await unitOfWork.Lists.Get(x => x.Id == emailSetupParams.ListId1, x => x.Name);



            // Send email
            await emailService.SendEmail(EmailType.RemovedCollaborator, "Removed from list", new Recipient
            {
                Email = recipient.email,
                FirstName = recipient.firstName,
                LastName = recipient.lastName
            }, new EmailProperties
            {
                Var1 = list
            });
        }







        // .........................................................................Is Duplicate.....................................................................
        private async Task<bool> IsDuplicate(int productId, string listId)
        {
            // Get all collaborator ids for the list
            IEnumerable<int> collaboratorIds = await unitOfWork.Collaborators.GetCollection(x => x.ListId == listId, x => x.Id);


            // Test to see if this product already exsist on this list
            int count = await unitOfWork.ListProducts.GetCount(x => collaboratorIds.Contains(x.CollaboratorId) && x.ProductId == productId);

            return count > 0;
        }










        // .........................................................................Add Product To List.....................................................................
        private async Task AddProductToList(int productId, string listId)
        {
            // Get the customer id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Get the collaborator id for this list
            int collaboratorId = await unitOfWork.Collaborators.Get(x => x.CustomerId == customerId && x.ListId == listId, x => x.Id);


            // Add the product to the list
            unitOfWork.ListProducts.Add(new ListProduct
            {
                ProductId = productId,
                CollaboratorId = collaboratorId,
                DateAdded = DateTime.Now
            });
        }




        // ..................................................................................Add List Product....................................................................
        [HttpPost]
        [Route("AddProduct")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> AddListProduct(NewListProduct newListProduct)
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Make sure this customer can add to this list
            if (!await unitOfWork.Collaborators.Any(x => (x.ListId == newListProduct.ListId && x.CustomerId == customerId && x.AddToList == true) || (x.ListId == newListProduct.ListId && x.CustomerId == customerId && x.IsOwner)))
            {
                return Unauthorized();
            }



            // Test to see if this product is already on this list
            bool isDuplicate = await IsDuplicate(newListProduct.ProductId, newListProduct.ListId);
            if (isDuplicate) return Ok(true);


            // Add the product to the list and save
            await AddProductToList(newListProduct.ProductId, newListProduct.ListId);
            await unitOfWork.Save();



            // Setup the email
            await SetupAddedListItemEmail(new EmailSetupParams
            {
                ListId1 = newListProduct.ListId,
                CustomerId = customerId,
                ProductId = newListProduct.ProductId,
                Host = GetHost()
            });


            return Ok();
        }






        // .........................................................................Remove List Product.....................................................................
        [Route("Product")]
        [HttpDelete]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> RemoveListProduct(int productId, int collaboratorId, string listId)
        {

            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Make sure this customer can add to this list
            if (!await unitOfWork.Collaborators.Any(x => (x.ListId == listId && x.CustomerId == customerId && x.RemoveItem) || (x.ListId == listId && x.CustomerId == customerId && x.IsOwner)))
            {
                return Unauthorized();
            }



            // Get the product to remove
            ListProduct listProduct = await unitOfWork.ListProducts.Get(x => x.ProductId == productId && x.CollaboratorId == collaboratorId);

            // Remove the product from the list and save
            unitOfWork.ListProducts.Remove(listProduct);
            await unitOfWork.Save();


            // Setup the email
            await SetupRemovedListItemEmail(new EmailSetupParams
            {
                ListId1 = listId,
                CustomerId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                ProductId = productId,
                Host = GetHost()
            });

            return Ok();
        }




        // .........................................................................Setup Removed List Item Email.....................................................................
        private async Task SetupRemovedListItemEmail(EmailSetupParams emailSetupParams)
        {
            List<string> recipientIds = await unitOfWork.Lists.GetRecipientIds(EmailType.RemovedListItem, emailSetupParams.ListId1, emailSetupParams.CustomerId);
            EmailParams emailParams = await unitOfWork.Lists.GetEmailParams(emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, recipientIds, emailSetupParams.ProductId);

            if (emailParams == null) return;


            // Send emails
            await SendRemovedListItemEmails(emailParams);
        }




        // .........................................................................Move List Product.....................................................................
        [Route("Product")]
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> MoveListProduct(MovedListProduct movedListProduct)
        {

            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Make sure this customer can add to this list
            if (!await unitOfWork.Collaborators.Any(x => (x.ListId == movedListProduct.FromListId && x.CustomerId == customerId && x.MoveItem) || (x.ListId == movedListProduct.FromListId && x.CustomerId == customerId && x.IsOwner)))
            {
                return Unauthorized();
            }



            // Test to see if this product is already on this list
            bool isDuplicate = await IsDuplicate(movedListProduct.ProductId, movedListProduct.ToListId);
            if (isDuplicate) return Ok(true);



            // Remove the product from the current list
            ListProduct listProduct = await unitOfWork.ListProducts.Get(x => x.ProductId == movedListProduct.ProductId && x.CollaboratorId == movedListProduct.CollaboratorId);
            unitOfWork.ListProducts.Remove(listProduct);



            // Add the product to the list
            await AddProductToList(movedListProduct.ProductId, movedListProduct.ToListId);



            // Save
            await unitOfWork.Save();




            // Setup the email
            await SetupMovedListItemEmail(new EmailSetupParams
            {
                ListId1 = movedListProduct.FromListId,
                ListId2 = movedListProduct.ToListId,
                CustomerId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                ProductId = movedListProduct.ProductId,
                Host = GetHost()
            });


            return Ok();
        }






        // .........................................................................Send Moved List Item Email.....................................................................
        private async Task SetupMovedListItemEmail(EmailSetupParams emailSetupParams)
        {
            IEnumerable<string> fromListCustomerIds = await unitOfWork.Collaborators.GetCollection(x => x.ListId == emailSetupParams.ListId1 && !x.IsRemoved && x.CustomerId != emailSetupParams.CustomerId && x.Customer.EmailPrefMovedListItem == true, x => x.CustomerId);


            List<string> bothListRecipientIds = await unitOfWork.Collaborators.GetCollection(x => x.ListId == emailSetupParams.ListId2 && !x.IsRemoved && x.CustomerId != emailSetupParams.CustomerId && fromListCustomerIds.Contains(x.CustomerId) && x.Customer.EmailPrefMovedListItem == true, x => x.CustomerId) as List<string>;


            if (bothListRecipientIds.Count() > 0)
            {
                EmailParams bothListEmailParams = await unitOfWork.Lists.GetEmailParams(emailSetupParams.CustomerId, emailSetupParams.ListId2, emailSetupParams.Host, bothListRecipientIds, emailSetupParams.ProductId);



                string fromListName = await unitOfWork.Lists.Get(x => x.Id == emailSetupParams.ListId1, x => x.Name);



                await emailService.SendEmail(EmailType.MovedListItem, "An item has been moved to another list", bothListEmailParams.Recipients, new EmailProperties
                {
                    Host = bothListEmailParams.Host,
                    Product = bothListEmailParams.Product,
                    Link = bothListEmailParams.Host + "/account/lists/" + bothListEmailParams.List.Id,
                    Person = bothListEmailParams.Collaborator,
                    Var1 = fromListName,
                    Var2 = bothListEmailParams.List.Name
                });
            }


            List<string> fromListRecipientIds = await unitOfWork.Lists.GetRecipientIds(EmailType.RemovedListItem, emailSetupParams.ListId1, emailSetupParams.CustomerId);
            fromListRecipientIds = fromListRecipientIds.Where(x => !bothListRecipientIds.Contains(x)).ToList();

            if (fromListRecipientIds.Count > 0)
            {
                EmailParams fromListEmailParams = await unitOfWork.Lists.GetEmailParams(emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, fromListRecipientIds, emailSetupParams.ProductId);
                await SendRemovedListItemEmails(fromListEmailParams);
            }





            List<string> toListRecipientIds = await unitOfWork.Lists.GetRecipientIds(EmailType.AddedListItem, emailSetupParams.ListId2, emailSetupParams.CustomerId);
            toListRecipientIds = toListRecipientIds.Where(x => !bothListRecipientIds.Contains(x)).ToList();

            if (toListRecipientIds.Count > 0)
            {
                EmailParams toListEmailParams = await unitOfWork.Lists.GetEmailParams(emailSetupParams.CustomerId, emailSetupParams.ListId2, emailSetupParams.Host, toListRecipientIds, emailSetupParams.ProductId);
                await SendAddedListItemEmails(toListEmailParams);
            }
        }





        // .........................................................................Send Removed List Item Emails.....................................................................
        private async Task SendRemovedListItemEmails(EmailParams emailParams)
        {
            await emailService.SendEmail(EmailType.RemovedListItem, "Item removed from list", emailParams.Recipients, new EmailProperties
            {
                Host = emailParams.Host,
                Product = emailParams.Product,
                Link = emailParams.Host + "/account/lists/" + emailParams.List.Id,
                Person = emailParams.Collaborator,
                Var1 = emailParams.List.Name
            });
        }




        // .........................................................................Setup Added List Item Email.....................................................................
        private async Task SetupAddedListItemEmail(EmailSetupParams emailSetupParams)
        {
            List<string> recipientIds = await unitOfWork.Lists.GetRecipientIds(EmailType.AddedListItem, emailSetupParams.ListId1, emailSetupParams.CustomerId);
            EmailParams emailParams = await unitOfWork.Lists.GetEmailParams(emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, recipientIds, emailSetupParams.ProductId);

            if (emailParams == null) return;


            // Send emails
            await SendAddedListItemEmails(emailParams);
        }







        // .........................................................................Send Added List Item Emails.....................................................................
        private async Task SendAddedListItemEmails(EmailParams emailParams)
        {
            await emailService.SendEmail(EmailType.AddedListItem, "Item added to list", emailParams.Recipients, new EmailProperties
            {
                Host = emailParams.Host,
                Product = emailParams.Product,
                Link = emailParams.Host + "/account/lists/" + emailParams.List.Id,
                Person = emailParams.Collaborator,
                Var1 = emailParams.List.Name
            });
        }














        // .........................................................................Get List Info.....................................................................
        [Route("ListInfo")]
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetListInfo(string collaborateId)
        {
            Customer customer = null;

            List list = await unitOfWork.Lists.Get(x => x.CollaborateId == collaborateId);

            if (list == null)
            {
                return NotFound();
            }




            bool exists = await unitOfWork.Collaborators.Any(x => x.CustomerId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.ListId == list.Id && !x.IsRemoved);






            if (!exists)
            {
                string customerId = await unitOfWork.Collaborators.Get(x => x.ListId == list.Id && x.IsOwner, x => x.CustomerId);
                customer = await unitOfWork.Customers.Get(customerId);
            }


            return Ok(new
            {
                listId = list.Id,
                ownerName = customer != null ? customer.FirstName + " " + customer.LastName : null,
                profilePic = customer != null ? customer.Image : null,
                listName = list.Name,
                exists
            });
        }









        // .........................................................................Add Collaborator.....................................................................
        [Route("Collaborator")]
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> AddCollaborator(ItemViewModel itemViewModel)
        {
            // Get the customer id
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Get the list
            var list = await unitOfWork.Lists.Get(x => x.CollaborateId == itemViewModel.Name, x => new
            {
                id = x.Id,
                name = x.Name
            });

            ListCollaborator collaborator = await unitOfWork.Collaborators.Get(x => x.CustomerId == customerId && x.ListId == list.id);

            if (collaborator == null)
            {
                // Add this customer to the list
                unitOfWork.Collaborators.Add(new ListCollaborator
                {
                    CustomerId = customerId,
                    ListId = list.id
                });
            }
            else
            {
                collaborator.IsRemoved = false;
                unitOfWork.Collaborators.Update(collaborator);
            }



            await unitOfWork.Save();


            // Setup the email
            await SetupAddedCollaboratorEmail(new EmailSetupParams
            {
                ListId1 = list.id,
                Host = GetHost(),
                CustomerId = customerId
            });




            return Ok();
        }












        // .........................................................................Setup Added Collaborator Email.....................................................................
        private async Task SetupAddedCollaboratorEmail(EmailSetupParams emailSetupParams)
        {


            List<string> recipientIds = await unitOfWork.Lists.GetRecipientIds(EmailType.NewCollaborator, emailSetupParams.ListId1, emailSetupParams.CustomerId);
            EmailParams emailParams = await unitOfWork.Lists.GetEmailParams(emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, recipientIds);

            if (emailParams == null) return;


            await emailService.SendEmail(EmailType.NewCollaborator, "A new Collaborator has joined your list", emailParams.Recipients, new EmailProperties
            {
                Link = emailParams.Host + "/account/lists/" + emailParams.List.Id,
                Person = emailParams.Collaborator,
                Var1 = emailParams.List.Name
            });
        }








        // ..................................................................................Get Host.....................................................................
        private string GetHost()
        {
            if (env.IsDevelopment())
            {
                return "http://localhost:4200";
            }

            return HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
        }
    }
}