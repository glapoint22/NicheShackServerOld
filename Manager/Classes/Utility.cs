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


       


        public enum BuilderType
        {
            Product,
            Page,
            Email
        }


        public enum MediaLocation
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
            ButtonWidgetBackground,
            VideoWidget
        }





        public enum NotificationType
        {
            UserName,
            UserImage,
            Message,
            Review,
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
            ProductSiteNolongerInService,
            MissingProductOther,
            Error
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
