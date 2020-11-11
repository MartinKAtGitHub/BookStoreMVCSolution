using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreMVC.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions _emailOptions;

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }
        public  Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            throw new NotImplementedException();
           // return Execute(_emailOptions.Key, subject, htmlMessage, email);
        }

        static async Task Execute(string sendGridKey, string subject, string message, string recipientEmail) // recipient 
        {
           //// var apiKey = Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
           // var client = new SendGridClient(sendGridKey);
           // var from = new EmailAddress("MartinAdmin@BookStoreMVC.com", "BookStoreMVC");
           // var to = new EmailAddress(recipientEmail, "End user");
           // //var plainTextContent = "and easy to do anywhere, even with C#";
           // var htmlContent = ""; //"<strong>and easy to do anywhere, even with C#</strong>";
           
           //// var msg = MailHelper.CreateSingleEmail(from, to, subject, message, htmlContent); // Correct way
           // var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlContent , message); // Looks better
           // var response = await  client.SendEmailAsync(msg);
        }

    }
}
