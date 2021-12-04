using System.Threading.Tasks;
using WCFService;
using WCFService.Services;

namespace WCFServiceTests1.Stubs
{
    internal class StubDiscord : INotificationSender
    {
        public string MostRecentMessage { get; private set; }

        public Task SendMessageAsync(string msg)
        {
            MostRecentMessage = msg;
            return Task.CompletedTask;
        }
    }
}