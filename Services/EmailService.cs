using DataAccess.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services
{
    public class EmailService
    {
        private readonly NicheShackContext context;
        private readonly IConfiguration configuration;

        public EmailService(NicheShackContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }



        // .........................................................................Send Email.....................................................................
        public async Task SendEmail(EmailType emailType, string subject, Recipient recipient, EmailProperties emailProperties = null)
        {
            List<Recipient> recipients = new List<Recipient>();
            recipients.Add(recipient);

            await SendEmail(emailType, subject, recipients, emailProperties);
        }




        public async Task SendEmail(EmailType emailType, string subject, IEnumerable<Recipient> recipients, EmailProperties emailProperties = null)
        {
            if(emailProperties == null)
            {
                emailProperties = new EmailProperties();
            }

            foreach (Recipient recipient in recipients)
            {
                emailProperties.Recipient = new Recipient
                {
                    FirstName = recipient.FirstName,
                    LastName = recipient.LastName,
                    Email = recipient.Email
                };

                EmailMessage emailMessage = new EmailMessage
                {
                    EmailType = emailType,
                    EmailAddress = recipient.Email,
                    Subject = subject,
                    EmailProperties = emailProperties
                };


                await SetUpEmail(emailMessage);
            }
        }

        private async Task SetUpEmail(EmailMessage emailMessage)
        {
            string emailContent = await GetEmailContent(emailMessage.EmailType);

            string emailBody = await GetEmailBody(emailContent, emailMessage.EmailProperties);

            MimeMessage email = GetEmail(emailMessage.EmailAddress, emailMessage.Subject, emailBody);

            Send(email);
        }




        private async Task<string> GetEmailContent(EmailType emailType)
        {
            string emailName = Regex.Replace(emailType.ToString(), "[A-Z]", " $0").Trim();
            return await context.Emails.Where(x => x.Name == emailName).Select(x => x.Content).SingleOrDefaultAsync();
        }


        private async Task<string> GetEmailBody(string content, EmailProperties emailProperties)
        {
            // Deserialize the content into an EmailPage object
            EmailPage emailPage = JsonSerializer.Deserialize<EmailPage>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });


            // Create the body
            string body = await emailPage.CreateBody(context);
            return emailProperties.Set(body);
        }



        private MimeMessage GetEmail(string recipient, string subject, string body)
        {
            MimeMessage email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(configuration["Email:Sender"]);
            email.To.Add(MailboxAddress.Parse(recipient));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            return email;
        }


        private void Send(MimeMessage email)
        {
            Task.Run(() => SendAsync(email));
        }

        private async Task SendAsync(MimeMessage email)
        {
            SmtpClient smtp = new SmtpClient();
            await smtp.ConnectAsync(configuration["Email:Host"], Convert.ToInt32(configuration["Email:Port"]), (SecureSocketOptions)Convert.ToInt32(configuration["Email:SecureSocketOption"]));
            await smtp.AuthenticateAsync(configuration["Email:UserName"], configuration["Email:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
