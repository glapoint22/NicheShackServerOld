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


        public void SetupEmail(Func<NicheShackContext, object, Task> func, object state)
        {
            emailSetupMethods.Add(new EmailSetupMethod
            {
                Func = func,
                Args = state
            });
        }
    }
}
