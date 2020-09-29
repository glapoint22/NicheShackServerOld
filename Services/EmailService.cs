using DataAccess.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
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



            string emailBody = emailPage.BuildEmail();

            emailBody = string.Format(emailBody, emailProperties.Host);




            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("no-reply@nicheshack.com");
            email.To.Add(MailboxAddress.Parse("to_address@example.com"));
            email.Subject = "Test Email Subject";
            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            // send email
            SmtpClient smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("chelsea.barton@ethereal.email", "hyf4dv5fcFzbVRjUWR");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);



        }


        
    }
}
