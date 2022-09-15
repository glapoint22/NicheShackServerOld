﻿using System;
using System.Collections.Generic;

namespace Manager.Classes
{

    public class TempProducts
    {
        public List<TempProduct> Products { get; set; }

        public static string GetProductData()
        {
        }
    }

    public class TempProduct
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public int ImageId { get; set; }
        public int NicheId { get; set; }
        public string UrlId { get; set; }
        public string UrlName { get; set; }
        public string Name { get; set; }
        public string Hoplink { get; set; }
        public string Description { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int TotalReviews { get; set; }
        public double Rating { get; set; }
        public int OneStar { get; set; }
        public int TwoStars { get; set; }
        public int ThreeStars { get; set; }
        public int FourStars { get; set; }
        public int FiveStars { get; set; }
        public DateTime Date { get; set; }
    }
}