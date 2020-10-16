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
using Website.ViewModels;
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



        // ..................................................................................List Exists......................................................................
        [HttpGet]
        [Authorize(Policy = "Account Policy")]
        [Route("ListExists")]
        public async Task<ActionResult> ListExists(string listId)
        {
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return Ok(await unitOfWork.Collaborators.Any(x => x.ListId == listId && x.CustomerId == customerId && !x.IsRemoved));
        }



        // ..................................................................................Get List Owner......................................................................
        [HttpGet]
        [Route("ListOwner")]
        public async Task<ActionResult> GetListOwner(string listId)
        {
            string ownerName = await unitOfWork.Collaborators.Get(x => x.ListId == listId && x.IsOwner, x => x.Customer.FirstName);

            if (ownerName != null)
            {
                return Ok(ownerName);
            }
            else
            {
                return NotFound();
            }


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





        // ..................................................................................Get Dropdown Lists......................................................................
        [HttpGet]
        [Route("DropdownLists")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> GetDropdownLists()
        {
            // Get the customer Id from the access token
            string customerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<string> listIds = await unitOfWork.Collaborators.GetCollection(x => x.CustomerId == customerId && !x.IsRemoved, x => x.ListId);


            // The customer's lists
            var lists = await unitOfWork.Collaborators.GetCollection(x => listIds.Contains(x.ListId) && x.IsOwner, x => new
            {
                id = x.ListId,
                name = x.List.Name + (x.CustomerId != customerId ? " (" + x.Customer.FirstName + ")" : string.Empty)
            });

            return Ok(lists);
        }





        // ..................................................................................Get Sort Options......................................................................
        [HttpGet]
        [Route("SortOptions")]
        public ActionResult GetSortOptions()
        {
            return Ok(new ListProductViewModel().GetSortOptions());
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
                    .GetCollection(x => x.ListId == listId && !x.IsRemoved, x => new Collaborator
                    {
                        Id = x.Id,
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
                    id = x.Id,
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
                listId = newList.Id
            });
        }









        // ..................................................................................Update List....................................................................
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> UpdateList(UpdatedList list)
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
                string previousName = updatedList.Name;

                updatedList.Name = list.Name;
                updatedList.Description = list.Description;

                // Update and save
                unitOfWork.Lists.Update(updatedList);
                await unitOfWork.Save();



                // Setup the email
                if (previousName != list.Name)
                {
                    emailService.SetupEmail(SetupChangedListName, new EmailSetupParams
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
        private async Task SetupChangedListName(NicheShackContext context, object state)
        {
            EmailSetupParams emailSetupParams = (EmailSetupParams)state;


            EmailParams emailParams = await GetEmailParams(context, emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host);

            if (emailParams == null) return;


            // Add email to queue
            emailService.AddToQueue(EmailType.ListNameChange, "List name changed", emailParams.Recipients, new EmailProperties
            {
                Host = emailSetupParams.Host,
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


                IEnumerable<string> customerIds = await unitOfWork.Collaborators.GetCollection(x => x.ListId == list.Id && !x.IsRemoved, x => x.CustomerId);

                if (customerIds.Count() > 1)
                {
                    IEnumerable<Recipient> recipients = await unitOfWork.Customers.GetCollection(x => customerIds.Contains(x.Id), x => new Recipient
                    {
                        CustomerId = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Email = x.Email
                    });



                    // Add email to queue
                    emailService.AddToQueue(EmailType.DeletedList, "List has been deleted", recipients.Where(x => x.CustomerId != customerId).ToList(), new EmailProperties
                    {
                        Host = GetHost(),
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






        // .........................................................................Remove Collaborator.....................................................................
        [Route("RemoveCollaborator")]
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> RemoveCollaborator(RemovedCollaborator removedCollaborator)
        {
            // Make sure we are the owner of the list
            if (!await unitOfWork.Collaborators.Any(x => x.ListId == removedCollaborator.ListId && x.CustomerId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.IsOwner))
            {
                return Unauthorized();
            }

            // Get the collaborator to remove
            ListCollaborator collaborator = await unitOfWork.Collaborators.Get(x => x.Id == removedCollaborator.Id && x.ListId == removedCollaborator.ListId);

            // If found, mark the collaborator as removed
            if (collaborator != null)
            {
                collaborator.IsRemoved = true;
                unitOfWork.Collaborators.Update(collaborator);
                await unitOfWork.Save();


                // Setup the email
                emailService.SetupEmail(SetupRemovedCollaborator, new EmailSetupParams
                {
                    Host = GetHost(),
                    CollaboratorId = collaborator.Id,
                    ListId1 = removedCollaborator.ListId
                });



                return Ok();
            }

            return NotFound();
        }






        // .........................................................................Setup Removed Collaborator.....................................................................
        private async Task SetupRemovedCollaborator(NicheShackContext context, object state)
        {
            EmailSetupParams emailSetupParams = (EmailSetupParams)state;

            // Get the customer id
            string customerId = await context.ListCollaborators
                .AsNoTracking()
                .Where(x => x.Id == emailSetupParams.CollaboratorId)
                .Select(x => x.CustomerId)
                .SingleAsync();



            // Get the recipient
            var recipient = await context.Customers
                .AsNoTracking()
                .Where(x => x.Id == customerId)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    email = x.Email
                }).SingleAsync();



            // Get the list
            string list = await context.Lists
                .AsNoTracking()
                .Where(x => x.Id == emailSetupParams.ListId1)
                .Select(x => x.Name).SingleAsync();



            // Add email to queue
            emailService.AddToQueue(EmailType.RemovedCollaborator, "Removed from list", new Recipient
            {
                Email = recipient.email,
                FirstName = recipient.firstName,
                LastName = recipient.lastName
            }, new EmailProperties
            {
                Host = emailSetupParams.Host,
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




        // ..................................................................................Add Product....................................................................
        [HttpPost]
        [Route("AddProduct")]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> AddProduct(NewListProduct newListProduct)
        {
            // Test to see if this product is already on this list
            bool isDuplicate = await IsDuplicate(newListProduct.ProductId, newListProduct.ListId);
            if (isDuplicate) return Ok(true);


            // Add the product to the list and save
            await AddProductToList(newListProduct.ProductId, newListProduct.ListId);
            await unitOfWork.Save();



            // Setup the email
            emailService.SetupEmail(SetupAddedListItemEmail, new EmailSetupParams
            {
                ListId1 = newListProduct.ListId,
                CustomerId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                ProductId = newListProduct.ProductId,
                Host = GetHost()
            });


            return Ok();
        }






        // .........................................................................Remove Product.....................................................................
        [Route("Product")]
        [HttpDelete]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> RemoveProduct(int productId, int collaboratorId, string listId)
        {
            // Get the product to remove
            ListProduct listProduct = await unitOfWork.ListProducts.Get(x => x.ProductId == productId && x.CollaboratorId == collaboratorId);

            // Remove the product from the list and save
            unitOfWork.ListProducts.Remove(listProduct);
            await unitOfWork.Save();


            // Setup the email
            emailService.SetupEmail(SetupRemovedListItemEmail, new EmailSetupParams
            {
                ListId1 = listId,
                CustomerId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                ProductId = productId,
                Host = GetHost()
            });

            return Ok();
        }






        // .........................................................................Move Product.....................................................................
        [Route("Product")]
        [HttpPut]
        [Authorize(Policy = "Account Policy")]
        public async Task<ActionResult> MoveProduct(MovedListProduct movedListProduct)
        {
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
            emailService.SetupEmail(SetupMovedListItemEmail, new EmailSetupParams
            {
                ListId1 = movedListProduct.FromListId,
                ListId2 = movedListProduct.ToListId,
                CustomerId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                ProductId = movedListProduct.ProductId,
                Host = GetHost()
            });


            return Ok();
        }





















        // .........................................................................Get Email Params.....................................................................
        private async Task<EmailParams> GetEmailParams(NicheShackContext context, string customerId, string listId, string host, int? productId = null)
        {
            // Get the recipients
            List<string> customerIds = await context.ListCollaborators
                .AsNoTracking()
                .Where(x => x.ListId == listId && x.CustomerId != customerId && !x.IsRemoved)
                .Select(x => x.CustomerId)
                .ToListAsync();

            // If we have no customer ids, return
            if (customerIds.Count == 0) return null;


            List<Recipient> recipients = await context.Customers
                .AsNoTracking()
                .Where(x => customerIds.Contains(x.Id))
                .Select(x => new Recipient
                {
                    CustomerId = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).ToListAsync();








            // Get the customer that did the action
            CustomerData collaborator = await context.Customers
                .AsNoTracking()
                .Where(x => x.Id == customerId)
                .Select(x => new CustomerData
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName
                }).SingleAsync();








            // Get the list
            ListViewModel list = await context.Lists
                .AsNoTracking()
                .Where(x => x.Id == listId)
                .Select(x => new ListViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).SingleAsync();







            // Get the product
            ProductData product = null;
            if (productId != null)
            {
                product = await context.Products
                .AsNoTracking()
                .Where(x => x.Id == productId)
                .Select(x => new ProductData
                {
                    Name = x.Name,
                    Image = x.Media.Url,
                    Url = host + "/" + x.UrlName + "/" + x.UrlId

                }).SingleAsync();
            }







            return new EmailParams
            {
                Collaborator = collaborator,
                List = list,
                Product = product,
                Recipients = recipients,
                Host = host
            };
        }





        // .........................................................................Setup Removed List Item Email.....................................................................
        private async Task SetupRemovedListItemEmail(NicheShackContext context, object state)
        {
            EmailSetupParams emailSetupParams = (EmailSetupParams)state;


            EmailParams emailParams = await GetEmailParams(context, emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, emailSetupParams.ProductId);

            if (emailParams == null) return;


            // Add emails to the queue
            SubmitRemovedListItemEmailsToQueue(emailParams);
        }




        // .........................................................................Submit Removed List Item Emails To Queue.....................................................................
        private void SubmitRemovedListItemEmailsToQueue(EmailParams emailParams)
        {
            emailService.AddToQueue(EmailType.RemovedListItem, "An item has been removed from your list", emailParams.Recipients, new EmailProperties
            {
                Host = emailParams.Host,
                Product = emailParams.Product,
                Link = emailParams.Host + "/account/lists/" + emailParams.List.Id,
                Person = emailParams.Collaborator,
                Var1 = emailParams.List.Name
            });
        }




        // .........................................................................Setup Added List Item Email.....................................................................
        private async Task SetupAddedListItemEmail(NicheShackContext context, object state)
        {
            EmailSetupParams emailSetupParams = (EmailSetupParams)state;


            EmailParams emailParams = await GetEmailParams(context, emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, emailSetupParams.ProductId);

            if (emailParams == null) return;


            // Add emails to the queue
            SubmitAddedListItemEmailsToQueue(emailParams);
        }







        // .........................................................................Submit Added List Item Emails To Queue.....................................................................
        private void SubmitAddedListItemEmailsToQueue(EmailParams emailParams)
        {
            emailService.AddToQueue(EmailType.AddedListItem, "An item has been added to your list", emailParams.Recipients, new EmailProperties
            {
                Host = emailParams.Host,
                Product = emailParams.Product,
                Link = emailParams.Host + "/account/lists/" + emailParams.List.Id,
                Person = emailParams.Collaborator,
                Var1 = emailParams.List.Name
            });
        }













        // .........................................................................Send Moved List Item Email.....................................................................
        private async Task SetupMovedListItemEmail(NicheShackContext context, object state)
        {
            EmailSetupParams emailSetupParams = (EmailSetupParams)state;


            EmailParams fromListEmailParams = await GetEmailParams(context, emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host, emailSetupParams.ProductId);
            EmailParams toListEmailParams = await GetEmailParams(context, emailSetupParams.CustomerId, emailSetupParams.ListId2, emailSetupParams.Host, emailSetupParams.ProductId);

            if (fromListEmailParams == null && toListEmailParams == null) return;


            if (fromListEmailParams != null && toListEmailParams == null)
            {
                SubmitRemovedListItemEmailsToQueue(fromListEmailParams);

            }
            else if (fromListEmailParams == null && toListEmailParams != null)
            {
                SubmitAddedListItemEmailsToQueue(toListEmailParams);
            }
            else
            {
                List<Recipient> bothListsRecipients = fromListEmailParams.Recipients.Where(x => toListEmailParams.Recipients.Select(z => z.CustomerId).ToList().Contains(x.CustomerId)).ToList();
                fromListEmailParams.Recipients = fromListEmailParams.Recipients.Where(x => !bothListsRecipients.Select(z => z.CustomerId).ToList().Contains(x.CustomerId)).ToList();
                toListEmailParams.Recipients = toListEmailParams.Recipients.Where(x => !bothListsRecipients.Select(z => z.CustomerId).ToList().Contains(x.CustomerId)).ToList();

                if (bothListsRecipients.Count > 0)
                {
                    emailService.AddToQueue(EmailType.MovedListItem, "An item has been moved to another list", bothListsRecipients, new EmailProperties
                    {
                        Host = fromListEmailParams.Host,
                        Product = fromListEmailParams.Product,
                        Link = fromListEmailParams.Host + "/account/lists/" + toListEmailParams.List.Id,
                        Person = fromListEmailParams.Collaborator,
                        Var1 = fromListEmailParams.List.Name,
                        Var2 = toListEmailParams.List.Name
                    });
                }

                SubmitRemovedListItemEmailsToQueue(fromListEmailParams);

                SubmitAddedListItemEmailsToQueue(toListEmailParams);
            }
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
                ownerName = customer != null ? customer.FirstName : null,
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
            emailService.SetupEmail(SetupAddedCollaboratorEmail, new EmailSetupParams
            {
                ListId1 = list.id,
                Host = GetHost(),
                CustomerId = customerId
            });




            return Ok();
        }












        // .........................................................................Setup Added Collaborator Email.....................................................................
        private async Task SetupAddedCollaboratorEmail(NicheShackContext context, object state)
        {
            EmailSetupParams emailSetupParams = (EmailSetupParams)state;

            EmailParams emailParams = await GetEmailParams(context, emailSetupParams.CustomerId, emailSetupParams.ListId1, emailSetupParams.Host);

            if (emailParams == null) return;


            emailService.AddToQueue(EmailType.NewCollaborator, "A new Collaborator has joined your list", emailParams.Recipients, new EmailProperties
            {
                Host = emailParams.Host,
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





    class EmailParams
    {
        public Person Collaborator { get; set; }
        public ListViewModel List { get; set; }
        public ProductData Product { get; set; }
        public IEnumerable<Recipient> Recipients { get; set; }
        public string Host { get; set; }
    }
}