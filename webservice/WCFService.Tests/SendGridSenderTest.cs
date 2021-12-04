using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCFService.Services;
using WCFServiceTests1.Stubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WCFServiceTests1
{
    [TestClass]
    public class SendGridSenderTest
    {
        [TestMethod]
        public void TestSendGridSenderImplementsINotificationSender()
        {
            SendGridSender a = null;
            INotificationSender b = a;
        }

        [TestMethod]
        public void TestConstructorTakesISendGridClient()
        {
            ISendGridClient sendGridClient = new StubSendGridClient();
            new SendGridSender(sendGridClient);
        }

        [TestMethod]
        public void TestSendMessageAsyncCallsSendGridClientSendEmailAsync()
        {
            // Arrange
            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient);

            // Act
            sender.SendMessageAsync("x").Wait();

            // Assert
            Assert.IsTrue(stubClient.SendEmailAsyncCalled);
        }

        [TestMethod]
        public async Task TestSendMessageAsyncPassesMessageToSendGridClientAsync()
        {
            // Arrange
            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient);
            var expectedMessage = "test message";

            // Act
            await sender.SendMessageAsync(expectedMessage);

            // Assert
            Assert.AreEqual(expectedMessage, stubClient.PlainTextContent);
        }

        [TestMethod]
        public async Task TestMailIsSentToConfiguredAddresses()
        {
            // Arrange
            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient);
            var expectedToAddresses = new List<EmailAddress>
            {
                new EmailAddress("bob@example.com", "Bob Example"),
                new EmailAddress("foo@example.net"),
            };

            // Act
            sender.ToAddresses = expectedToAddresses;
            await sender.SendMessageAsync("x");

            // Assert
            var expected = expectedToAddresses;
            var actual = stubClient.ToAddresses;
            var expectedNotActual = expected.Except(actual);
            var actualNotExpected = actual.Except(expected);

            Assert.IsFalse(expectedNotActual.Any() && actualNotExpected.Any());
        }

        [TestMethod]
        public async Task TestSentMailHasConfiguredSubject()
        {
            var expectedSubject = "configured subject";

            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient)
            {
                Subject = expectedSubject,
            };

            await sender.SendMessageAsync("x");

            Assert.AreEqual(expectedSubject, stubClient.Subject);
        }

        [TestMethod]
        public async Task TestSentMailHasConfiguredFromAddress()
        {
            var expectedFrom = new EmailAddress("from-address@example.org");

            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient)
            {
                FromAddress = expectedFrom,
            };

            await sender.SendMessageAsync("x");

            Assert.AreEqual(expectedFrom, stubClient.FromAddress);
        }

        [TestMethod]
        public async Task TestMailHtmlContentIsPlainTextContentWrappedInParagraphTag()
        {
            var originalMessage = "the message";
            var expectedHtmlContent = $"<p>{originalMessage}</p>";

            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient);

            await sender.SendMessageAsync(originalMessage);

            Assert.AreEqual(expectedHtmlContent, StubSendGridClient.HtmlContent);
        }

        [TestMethod]
        public async Task TestSendMessageAsyncThrowsArgumentExceptionOnNullMsg()
        {
            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient);
            string msg = null;
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await sender.SendMessageAsync(msg));
        }

        [TestMethod]
        public async Task TestSendMessageAsyncThrowsArgumentExceptionOnEmptyMsg()
        {
            var stubClient = new StubSendGridClient();
            var sender = new SendGridSender(stubClient);
            var msg = "";
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () => await sender.SendMessageAsync(msg));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException))]
        public void TestSendGridSenderConstructorThrowsArgumentNullExceptionIfGivenNullISendGridClient()
        {
            new SendGridSender(null);
        }
    }
}
