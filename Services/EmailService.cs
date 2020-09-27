using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Services.Classes;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services
{
    public class EmailService
    {
        private readonly NicheShackContext context;

        public EmailService(NicheShackContext context)
        {
            this.context = context;
        }


        public async Task SendEmail(EmailProperties emailProperties)
        {
            string emailName = Regex.Replace(emailProperties.EmailType.ToString(), "[A-Z]", " $0").Trim();

            string content = await context.Emails.Where(x => x.Name == emailName).Select(x => x.Content).SingleOrDefaultAsync();

            

            EmailPage emailPage = JsonSerializer.Deserialize<EmailPage>(content, new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true
            });



            string email = emailPage.BuildEmail();

            email = string.Format(email, emailProperties.Host);
        }


        
    }
}
