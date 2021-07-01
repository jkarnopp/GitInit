using GitInitTest.Common.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using EmailAddress = SendGrid.Helpers.Mail.EmailAddress;

namespace GitInitTest.Site.Services
{
    public class EmailSender : IEmailSender
    {
        public EmailSender(ISendGridConfiguration sendGridConfiguration)
        {
            _sendGridApiKey = sendGridConfiguration.SendGridApiKey;
        }

        private readonly string _sendGridApiKey;

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(_sendGridApiKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("NoReply@workmasterpro.com", "GitInitTest.Site Support"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
    }
}