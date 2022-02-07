using System.ComponentModel.DataAnnotations;

namespace Website.Classes
{
    public class ResetPassword
    {
        [EmailAddress]
        public string Email { get; set; }
        [Password]
        public string Password { get; set; }
        public string OneTimePassword { get; set; }

    }
}
