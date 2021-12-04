using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WCFServiceTests1.Stubs
{
    internal class StubSendGridClient : ISendGridClient
    {
        public string UrlPath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Version { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string MediaType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public AuthenticationHeaderValue AddAuthorization(KeyValuePair<string, string> header)
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> MakeRequest(HttpRequestMessage request, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> RequestAsync(SendGridClient.Method method, string requestBody = null, string queryParams = null, string urlPath = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new System.NotImplementedException();
        }

        public Task<Response> SendEmailAsync(SendGridMessage msg, CancellationToken cancellationToken = default(CancellationToken))
        {
            SendEmailAsyncCalled = true;
            if (msg?.Personalizations != null && msg.Personalizations.Count >= 1)
            {
                ToAddresses = new List<EmailAddress>();
                foreach (var p in msg.Personalizations)
                {
                    ToAddresses.AddRange(p.Tos);
                }
            }
            Subject = msg.Subject;
            FromAddress = msg.From;

            var plainTextContents = msg.Contents.Where(c => c.Type == "text/plain");
            if (plainTextContents.Count() > 1)
            {
                throw new ArgumentException("more than one plain text content", nameof(msg));
            }
            PlainTextContent = plainTextContents.First().Value;

            var htmlContents = msg.Contents.Where(c => c.Type == "text/html");
            if (htmlContents.Count() > 1)
            {
                throw new ArgumentException("more than one html text content", nameof(msg));
            }
            HtmlContent = htmlContents.First().Value;

            return Task.FromResult<Response>(null);
        }

        public bool SendEmailAsyncCalled { get; private set; } = false;
        public List<EmailAddress> ToAddresses { get; private set; }
        public string Subject { get; private set; }
        public EmailAddress FromAddress { get; private set; }
        public string PlainTextContent { get; private set; }
        public static string HtmlContent { get; internal set; }
    }
}