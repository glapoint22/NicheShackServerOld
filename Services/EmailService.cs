using DataAccess.Models;
using Services.Classes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class EmailService
    {
        public List<EmailMessage> emails = new List<EmailMessage>();
        public List<EmailSetupMethod> emailSetupMethods = new List<EmailSetupMethod>();


        // .........................................................................Setup Email.....................................................................
        public void SetupEmail(Func<NicheShackContext, object, Task> func, object state)
        {
            emailSetupMethods.Add(new EmailSetupMethod
            {
                Func = func,
                Args = state
            });
        }






        // .........................................................................Add To Queue.....................................................................
        public void AddToQueue(EmailType emailType, string subject, Recipient recipient, EmailProperties emailProperties)
        {
            List<Recipient> recipients = new List<Recipient>();
            recipients.Add(recipient);

            AddToQueue(emailType, subject, recipients, emailProperties);
        }




        public void AddToQueue(EmailType emailType, string subject, IEnumerable<Recipient> recipients, EmailProperties emailProperties)
        {
            foreach (Recipient recipient in recipients)
            {
                emailProperties.Recipient = new Recipient
                {
                    FirstName = recipient.FirstName,
                    LastName = recipient.LastName,
                    Email = recipient.Email
                };

                emails.Add(new EmailMessage
                {
                    EmailType = emailType,
                    EmailAddress = recipient.Email,
                    Subject = subject,
                    EmailProperties = emailProperties
                });
            }
        }
    }
}
