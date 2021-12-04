using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WCFService.Services
{
    public class SendGridSender : INotificationSender
    {
        private readonly ISendGridClient _sendGrid;

        public SendGridSender(ISendGridClient sendGrid)
        {
            _sendGrid = sendGrid ?? throw new ArgumentNullException(nameof(sendGrid));
            ToAddresses = new List<EmailAddress>();
        }

        public async Task SendMessageAsync(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                throw new ArgumentException($"{nameof(msg)} null or empty", nameof(msg));
            }
            var htmlMsg = $"<p>{msg}</p>";
            var mail = MailHelper.CreateSingleEmailToMultipleRecipients(FromAddress, ToAddresses, Subject, msg, htmlMsg);
            await _sendGrid.SendEmailAsync(mail);
        }

        public EmailAddress FromAddress { get; set; }
        public List<EmailAddress> ToAddresses { get; set; }
        public string Subject { get; set; }
    }
}