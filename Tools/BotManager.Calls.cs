using Calls.Api.Bots;
using System;
using System.Linq;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Chat = Calls.Entities.Json.Chat;

namespace Calls.Bot.Services
{
    public partial class BotManager
    {
        public async Task SendGroupDayTimeReport()
        {
            var now = DateTime.UtcNow;

            foreach (var chat in bot.Chats.Where(c => c.IsGroup))
                await GetUniqueCallsCount(now, 1, bot, chat, cancellationTokenSource.Token);
        }

        private Task GetUniqueTodayCallsCount(BotBase bot, Chat chat, string _, CancellationToken token)
            => GetUniqueCallsCount(DateTime.UtcNow, 1, bot, chat, token);

        private async Task GetUniqueCallsCount(DateTime utcNow, int v, BotBase bot, Chat chat, CancellationToken token)
        {
            var messages = await callsManager.GetUniqueCallsMessage(true, true, true, token);

            foreach (var message in messages)
                await bot.SendMessage(chat.ChatId, message);
        }
    }
}
