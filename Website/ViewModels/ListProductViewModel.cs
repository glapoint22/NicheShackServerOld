﻿using System.Collections.Generic;
using System.Linq;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Website.ViewModels
{
    public class ListProductViewModel : ISort<ListProduct>
    {
        private readonly string sortBy;

        public int Id { get; set; }
        public string UrlId { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public string DateAdded { get; set; }
        public CollaboratorViewModel Collaborator { get; set; }
        public string Hoplink { get; set; }
        public ImageViewModel Image { get; set; }
        public string UrlTitle { get; set; }

        // Constructors
        public ListProductViewModel() { }

        public ListProductViewModel(string sortBy)
        {
            this.sortBy = sortBy;
        }


        // .............................................................................Get Sort Options.....................................................................
        public List<KeyValuePair<string, string>> GetSortOptions()
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Date Added", "date"),
                new KeyValuePair<string, string>("Price: Low to High", "price-asc"),
                new KeyValuePair<string, string>("Price: High to Low", "price-desc"),
                new KeyValuePair<string, string>("Highest Rating", "rating"),
                new KeyValuePair<string, string>("Title", "title")
            };
        }



        // .............................................................................Set Sort Option.....................................................................
        public IOrderedQueryable<ListProduct> SetSortOption(IQueryable<ListProduct> source)
        {
            IOrderedQueryable<ListProduct> sortResult = null;


            switch (sortBy)
            {
                case "price-asc":
                    sortResult = source.OrderBy(x => x.Product.MinPrice);
                    break;
                case "price-desc":
                    sortResult = source.OrderByDescending(x => x.Product.MinPrice);
                    break;
                case "rating":
                    sortResult = source.OrderByDescending(x => x.Product.Rating);
                    break;
                case "title":
                    sortResult = source.OrderBy(x => x.Product.Name);
                    break;
                default:
                    sortResult = source.OrderByDescending(x => x.DateAdded);
                    break;
            }

            return sortResult;
        }
    }
}
