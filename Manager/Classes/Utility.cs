using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manager.Classes
{
    public class Utility
    {
        public enum MediaType
        {
            Image,
            BackgroundImage,
            BannerImage,
            CategoryImage,
            ProductImage,
            Icon,
            Video,
            Search,
            ProductMediaImage
        }




        public enum NotificationType
        {
            Message,
            ReviewComplaint,
            ProductNameDoesNotMatchWithProductDescription,
            ProductNameDoesNotMatchWithProductImage,
            ProductNameOther,
            ProductPriceTooHigh,
            ProductPriceNotCorrect,
            ProductPriceOther,
            VideosAndImagesAreDifferentFromProduct,
            NotEnoughVideosAndImages,
            VideosAndImagesNotClear,
            VideosAndImagesMisleading,
            VideosAndImagesOther,
            ProductDescriptionIncorrect,
            ProductDescriptionTooVague,
            ProductDescriptionMisleading,
            ProductDescriptionOther,
            ProductReportedAsIllegal,
            ProductReportedAsHavingAdultContent,
            OffensiveProductOther,
            ProductInactive,
            ProductSiteNoLongerInService,
            MissingProductOther
        }


        public static string GetNotificationName(int type)
        {
            string name = string.Empty;

            switch (type)
            {
                case 0:
                    name = "Message";
                    break;

                case 1:
                    name = "Review Complaint";
                    break;

                case 2:
                    name = "Product Name Doesn\'t Match With Product Description";
                    break;

                case 3:
                    name = "Product Name Doesn\'t Match With Product Image";
                    break;

                case 4:
                    name = "Product Name (Other)";
                    break;

                case 5:
                    name = "Product Price Too High";
                    break;

                case 6:
                    name = "Product Price Not Correct";
                    break;

                case 7:
                    name = "Product Price (Other)";
                    break;

                case 8:
                    name = "Videos & Images are Different From Product";
                    break;

                case 9:
                    name = "Not Enough Videos & Images";
                    break;

                case 10:
                    name = "Videos & Images Not Clear";
                    break;

                case 11:
                    name = "Videos & Images Misleading";
                    break;

                case 12:
                    name = "Videos & Images (Other)";
                    break;

                case 13:
                    name = "Product Description Incorrect";
                    break;

                case 14:
                    name = "Product Description Too Vague";
                    break;

                case 15:
                    name = "Product Description Misleading";
                    break;

                case 16:
                    name = "Product Description (Other)";
                    break;

                case 17:
                    name = "Product Reported As Illegal";
                    break;

                case 18:
                    name = "Product Reported As Having Adult Content";
                    break;

                case 19:
                    name = "Offensive Product (Other)";
                    break;


                case 20:
                    name = "Product Inactive";
                    break;

                case 21:
                    name = "Product site no longer in service";
                    break;


                case 22:
                    name = "Missing Product (Other)";
                    break;
            }

            return name;
        }



        public static string GetUrlName(string name)
        {
            return Regex.Replace(name, @"[^a-zA-Z0-9\-]", evaluator);
        }


        private static string evaluator(Match match)
        {
            Match nextMatch = match.NextMatch();
            if (nextMatch.Index == match.Index + 1) return "";
            return "-";
        }
    }
}
