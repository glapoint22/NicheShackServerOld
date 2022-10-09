namespace Website.Classes
{
    public struct Enums
    {
        public enum ProductMediaType
        {
            Video,
            Image
        }

        public enum PaymentMethod
        {
            PYPL,
            VISA,
            MSTR,
            DISC,
            AMEX,
            SOLO,
            DNRS,
            MAES,
            TEST
        }


        public enum OtpType
        {
            ActivateAccount,
            EmailChange,
            DeleteAccount,
            ResetPassword
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
    }
}
