using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using SendGrid;
using SendGrid.Helpers.Mail;
using WCFService.DataLayer;
using WCFService.Services;

namespace WCFService
{
    public class Global : System.Web.HttpApplication
    {
        public static DiscordSender Bot { get; set; }

        public static List<INotificationSender> NotificationSenders { get; set; } = new List<INotificationSender>();

        protected void Application_Start(object sender, EventArgs e)
        {
            var discordToken = ConfigurationManager.AppSettings["DiscordToken"];
            var discordBot = new DiscordSender(discordToken);
            discordBot.StartAsync().Wait();
            NotificationSenders.Add(discordBot);

            var sendGridToken = ConfigurationManager.AppSettings["SendGridToken"];
            var sendGridClient = new SendGridClient(sendGridToken);
            var sendGrid = new SendGridSender(sendGridClient)
            {
                FromAddress = new EmailAddress("tons@tons.cgt.name", "TONS Notifier"),
                Subject = "TONS: Notifikation om overophedning!",
            };
            var addrRepo = new SqlNotifyEmailAddressRepository(AppConfig.Instance.ConnectionString);
            sendGrid.ToAddresses = addrRepo.GetAll().Select(addr => new EmailAddress(addr)).ToList();
            NotificationSenders.Add(sendGrid);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}