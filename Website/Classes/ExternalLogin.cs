namespace Website.Classes
{
    public class ExternalLogin
    {
        public string Provider { get; }
        public bool HasPassword { get; }

        public ExternalLogin(string Provider, bool HasPassword)
        {
            this.Provider = Provider;
            this.HasPassword = HasPassword;
        }
    }
}
