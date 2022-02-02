using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Classes;
using DataAccess.Models;
using DataAccess.Repositories;
using DataAccess.Classes;
using Website.ViewModels;
using Services.Classes;
using System.Linq.Expressions;

namespace Website.Repositories
{
    public class ListRepository : Repository<List>, IListRepository
    {
        // Set the context
        private readonly NicheShackContext context;

        public ListRepository(NicheShackContext context) : base(context)
        {
            this.context = context;
        }




        // ................................................................................Get Lists.....................................................................
        public async Task<IEnumerable<ListViewModel>> GetLists(string customerId)
        {
            // Get the list ids
            var listCollaborators = await context.ListCollaborators
                .AsNoTracking()
                .OrderByDescending(x => x.IsOwner)
                .Where(x => x.CustomerId == customerId && !x.IsRemoved)
                .Select(x => new
                {
                    x.ListId,
                    x.IsOwner,
                    x.AddToList,
                    x.ShareList,
                    x.InviteCollaborators,
                    x.EditList,
                    x.DeleteList,
                    x.MoveItem,
                    x.RemoveItem
                })
                .ToListAsync();

            // Get the list info
            var lists = await context.Lists
                .AsNoTracking()
                .Where(x => listCollaborators.Select(z => z.ListId).ToList().Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Description,
                    x.CollaborateId,
                    CollaboratorCount = x.Collaborators
                        .Count(y => y.ListId == x.Id && !y.IsOwner && !y.IsRemoved),
                    Owner = x.Collaborators
                        .Where(y => y.ListId == x.Id && y.IsOwner)
                        .Select(y => new
                        {
                            y.Id,
                            Name = y.CustomerId == customerId ? "You" : y.Customer.FirstName,
                            ProfilePic = new ProfilePicInfo
                            {
                                Name = y.Customer.FirstName + " " + y.Customer.LastName,
                                Url = y.Customer.Image != null ? "images/" + y.Customer.Image : "assets/no-account-pic.png"
                            }

                        }).FirstOrDefault(),
                    TotalItems = context.ListProducts
                        .Count(z => x.Collaborators
                            .Select(y => y.Id)
                            .Contains(z.CollaboratorId))

                }).ToListAsync();


            // Returns all of the customer's lists
            return listCollaborators.Join(lists, x => x.ListId, x => x.Id, (x, y) => new
            {
                y.Id,
                y.Name,
                y.Description,
                y.TotalItems,
                Owner = y.Owner.Name,
                y.CollaborateId,
                ProfilePic = y.Owner.ProfilePic,
                x.IsOwner,
                y.CollaboratorCount,
                x.AddToList,
                x.ShareList,
                x.InviteCollaborators,
                x.EditList,
                x.DeleteList,
                x.MoveItem,
                x.RemoveItem
            })
                .OrderByDescending(x => x.IsOwner)
                .Select(x => new ListViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    TotalItems = x.TotalItems,
                    OwnerName = x.Owner,
                    IsOwner = x.IsOwner,
                    CollaborateId = x.CollaborateId,
                    ProfilePic = x.ProfilePic,
                    CollaboratorCount = x.CollaboratorCount,
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
                }).ToList();
        }






        // ................................................................................Get List Products.....................................................................
        public async Task<IEnumerable<ListProductViewModel>> GetListProducts(IEnumerable<int> collaboratorIds, string customerId, string sort)
        {
            // Gets products based on collaborators from a list.
            return await context.ListProducts
                .AsNoTracking()
                .OrderBy(new ListProductViewModel(sort))
                .Where(x => collaboratorIds
                    .Contains(x.CollaboratorId))
                .Select(x => new ListProductViewModel
                {
                    Id = x.Product.Id,
                    UrlId = x.Product.UrlId,
                    Name = x.Product.Name,
                    Rating = x.Product.Rating,
                    TotalReviews = x.Product.TotalReviews,
                    MinPrice = x.Product.MinPrice,
                    MaxPrice = x.Product.MaxPrice,
                    DateAdded = x.DateAdded.ToString("MMMM dd, yyyy"),
                    Collaborator = new CollaboratorViewModel
                    {
                        Id = x.CollaboratorId,
                        Name = x.Collaborator.CustomerId == customerId ? "you" : x.Collaborator.Customer.FirstName,
                        Image = new ProfilePicInfo
                        {
                            Url = x.Collaborator.Customer.Image,
                            Name = x.Collaborator.Customer.FirstName + " " + x.Collaborator.Customer.LastName
                        }
                    },
                    Hoplink = x.Product.Hoplink + (customerId != null ? (x.Product.Hoplink.Contains('?') ? "&" : "?") + "tid=" + x.Product.UrlId + "_" + customerId : ""),
                    Image = new ImageViewModel
                    {
                        Name = x.Product.Media.Name,
                        Url = x.Product.Media.Image
                    },
                    UrlName = x.Product.UrlName,
                    OneStar = x.Product.OneStar,
                    TwoStars = x.Product.TwoStars,
                    ThreeStars = x.Product.ThreeStars,
                    FourStars = x.Product.FourStars,
                    FiveStars = x.Product.FiveStars
                })
                .ToListAsync();
        }






        // ................................................................................Get Recipient Ids.....................................................................
        public async Task<List<string>> GetRecipientIds(EmailType emailType, string listId, string customerId)
        {
            Expression<Func<ListCollaborator, bool>> predicate = null;

            switch (emailType)
            {
                case EmailType.NewCollaborator:
                    predicate = x => x.Customer.EmailPrefNewCollaborator == true;
                    break;
                case EmailType.RemovedListItem:
                    predicate = x => x.Customer.EmailPrefRemovedListItem == true;
                    break;
                case EmailType.AddedListItem:
                    predicate = x => x.Customer.EmailPrefAddedListItem == true;
                    break;
                case EmailType.ListNameChange:
                    predicate = x => x.Customer.EmailPrefListNameChange == true;
                    break;
            }

            return await context.ListCollaborators
                .AsNoTracking()
                .Where(x => x.ListId == listId && x.CustomerId != customerId && !x.IsRemoved)
                .Where(predicate)
                .Select(x => x.CustomerId)
                .ToListAsync();
        }













        // ................................................................................Get Email Params.....................................................................
        public async Task<EmailParams> GetEmailParams(string customerId, string listId, string host, List<string> recipientIds, int? productId = null)
        {
            List<Recipient> recipients = await context.Customers
                .AsNoTracking()
                .Where(x => recipientIds.Contains(x.Id))
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
                    Image = x.Media.Image,
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











        // ................................................................................First List.....................................................................
        public async Task<string> FirstList(string customerId)
        {
            var lists = await context.ListCollaborators
                .AsNoTracking()
                .OrderByDescending(x => x.IsOwner)
                .Where(x => x.CustomerId == customerId && !x.IsRemoved)
                .Select(x => x.ListId)
                .ToListAsync();

            return lists.FirstOrDefault();
        }
    }
}
