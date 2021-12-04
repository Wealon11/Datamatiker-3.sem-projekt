using System.Threading.Tasks;

namespace WCFService.Services
{
    public interface INotificationSender
    {
        Task SendMessageAsync(string msg);
    }
}