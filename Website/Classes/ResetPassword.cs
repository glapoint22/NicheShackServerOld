using System.ComponentModel.DataAnnotations;

namespace Website.Classes
{
    public class ResetPassword
    {
        public string Token { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Password]
        public string Password { get; set; }
    }
}
