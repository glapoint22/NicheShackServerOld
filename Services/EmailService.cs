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


        public async Task SendEmail(EmailType emailType, string recipient, string subject, EmailProperties emailProperties)
        {
            string emailName = Regex.Replace(emailType.ToString(), "[A-Z]", " $0").Trim();
            string content = await context.Emails.Where(x => x.Name == emailName).Select(x => x.Content).SingleOrDefaultAsync();

            // Deserialize the content into an EmailPage object
            EmailPage emailPage = JsonSerializer.Deserialize<EmailPage>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


            // Create the body
            string body = emailPage.CreateBody();
            body = emailProperties.Set(body);


            // Setup the email
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("no-reply@nicheshack.com");
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            // Send email
            SmtpClient smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("chelsea.barton@ethereal.email", "hyf4dv5fcFzbVRjUWR");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
