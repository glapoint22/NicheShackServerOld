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
            EmailChange,
            AccountDeletion
        }
    }
}
