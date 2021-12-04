using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.WebSocket;

namespace WCFService.Services
{
    public class DiscordSender : INotificationSender
    {
        private DiscordSocketClient _client;
        private readonly string _discordToken;

        public DiscordSender(string discordToken)
        {
            _discordToken = discordToken;
            _client = new DiscordSocketClient();
        }

        public async Task StartAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _discordToken);
            await _client.StartAsync();
        }

        public async Task SendMessageAsync(string msg)
        {
            await ((ISocketMessageChannel)_client.GetChannel(442953876430127105)).SendMessageAsync(msg);
        }
    }
}
