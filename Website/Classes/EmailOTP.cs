using System.ComponentModel.DataAnnotations;

namespace Website.Classes
{
    public class EmailOTP : OTP
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}