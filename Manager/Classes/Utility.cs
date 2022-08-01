using System;
using System.Text.RegularExpressions;

namespace Manager.Classes
{
    public class Utility
    {
        public enum MediaType
        {
            Image,
            Video
        }



        public enum VideoType
        {
            YouTube,
            Vimeo,
            Wistia
        }


        public enum PageType
        {
            Custom,
            Home,
            Browse,
            Search,
            Grid
        }




        public enum ImageSizeType
        {
            AnySize = 0,
            Thumbnail = 100,
            Small = 200,
            Medium = 500,
            Large = 675
        }


        public enum BuilderType
        {
            Product,
            Page,
            Email
        }


        public enum ImageLocation
        {
            Product,
            Media,
            PricePoint,
            Component,
            Bonus,
            PageBackground,
            RowBackground,
            ColumnBackground,
            ImageWidget,
            CarouselWidgetBanner,
            TextWidgetBackground,
            ContainerWidgetBackground,
            ButtonWidgetBackground
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
            name = name.Replace("'", "");
            name = Regex.Replace(name, @"^[\W]|[\W]$", "");
            return Regex.Replace(name, @"[\W_]+", "-");
        }


        public static string GetUrlId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        }
    }
}
