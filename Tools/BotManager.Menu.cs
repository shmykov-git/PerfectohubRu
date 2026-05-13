using Calls.Api.Bots;
using Shared.Model.Enums;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chat = Calls.Entities.Json.Chat;

namespace Calls.Bot.Services
{
    public delegate Task MenuAction(BotBase botBase, Chat chat, string message, CancellationToken token);

    public partial class BotManager // Menu
    {
        private (string name, MenuAction action)[][] GetActions(BotBase bot) =>
            new (string name, MenuAction action)[][]
            {
                new (string name, MenuAction action)[]
                {
                    ("📞 Сегодня", GetUniqueTodayCallsCount), ("ℹ️ О боте", GetAbout),
                }
            };

        private async Task GetAbout(BotBase bot, Chat chat, string _, CancellationToken token)
        {
            var o = clientOptions;

            var aboutMessage = $@"
Я собираю статистику с ATS, затем ее анализирую.
<b>Периодически</b> я собираю отчеты по звонкам за текущий день, чтобы помочь <b>не потерять клиентов</b>, которые не смогли дозвониться

Я использую значки в своих записях:
    {o.Marks[MarkType.Lock]} - это постоянный клиент, который уже звонил в компанию месяц тому назад (только в серверной pro-версии)
    {o.Marks[MarkType.Repeat]} - кто-то из сотрудников пытался дозвониться до клиента, но не вышло
    {o.Marks[MarkType.Inbound]} - звонок прошел через общий номер АТС
|10:45 - с этим клиентом недавно общались, но он снова позвонил

Последний раз меня запустили <b>{StartTime.ToString("dd.MM.yyyy")}</b>
Меня разрабатывает компания <b>Perfecto (perfectohub.ru)</b>
    ";

            await bot.SendMessage(chat.ChatId, aboutMessage);
        }

        public string[][] GetKeyboard(BotBase bot) => GetActions(bot).Select(line => line.Select(a => a.name).ToArray()).ToArray();

        public MenuAction GetAction(BotBase bot, string name)
        {
            var spamMark = bot.Marks[MarkType.Stop];
            var notSpamMark = bot.Marks[MarkType.CheckLight];

            return GetActions(bot).SelectMany(v => v).Where(v => v.name == name).Select(v => v.action).FirstOrDefault();
        }
    }
}
