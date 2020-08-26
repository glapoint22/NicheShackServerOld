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
            // Returns all of the customer's lists.
            return await context.ListCollaborators
                .AsNoTracking()
                .OrderByDescending(x => x.IsOwner)
                .Where(x => x.CustomerId == customerId)
                .Select(x => new ListViewModel
                {
                    Id = x.ListId,
                    Name = x.List.Name,
                    Description = x.List.Description,
                    TotalItems = context.ListProducts
                        .Count(z => x.List.Collaborators
                            .Select(y => y.Id)
                            .Contains(z.CollaboratorId)),
                    Owner = x.List.Collaborators
                        .Where(y => y.ListId == x.ListId && y.IsOwner)
                        .Select(y => y.CustomerId == customerId ? "You" : y.Customer.FirstName)
                        .FirstOrDefault(),
                    CollaborateId = x.List.CollaborateId
                })
                .ToListAsync();
        }






        // ................................................................................Get List Products.....................................................................
        public async Task<IEnumerable<ListProductViewModel>> GetListProducts(IEnumerable<int> collaboratorIds, string customerId, string sort)
        {
            // Gets products based on collaborators from a list.
            var products = await context.ListProducts
                .AsNoTracking()
                .SortBy(new ListProductViewModel(sort))
                .Where(x => collaboratorIds
                    .Contains(x.CollaboratorId))
                .Select(x => new ListProductViewModel
                {
                    UrlId = x.Product.UrlId,
                    Title = x.Product.Name,
                    Rating = x.Product.Rating,
                    TotalReviews = x.Product.TotalReviews,
                    MinPrice = x.Product.MinPrice,
                    MaxPrice = x.Product.MaxPrice,
                    DateAdded = x.DateAdded.ToString("MMMM dd, yyyy"),
                    Collaborator = x.Collaborator.CustomerId == customerId ? "you" : x.Collaborator.Customer.FirstName,
                    Hoplink = x.Product.Hoplink,
                    Image = new ImageViewModel
                    {
                        Name = x.Product.Media.Name,
                        Url = x.Product.Media.Url
                    },
                    UrlTitle = x.Product.UrlName
                })
                .ToListAsync();

            return products;
        }
    }
}
