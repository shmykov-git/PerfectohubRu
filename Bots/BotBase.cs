using Calls.Bot.Services;
using Calls.Entities.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Perfecto.Deploy.Extensions;
using Shared.Model.Enums;
using Shared.Model.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Calls.Api.Bots
{
    public abstract class BotBase
    {
        protected CancellationToken cancellationToken;
        protected BotManager botManager;
        protected ClientOptions clientOptions;
        private readonly IServiceProvider sp;
        protected IConfiguration configuration;
        protected string ClientId { get; private set; }

        protected Dictionary<string, Chat> chats = new Dictionary<string, Chat>();

        protected bool IsStarted { get; set; }
        public IEnumerable<Chat> Chats => chats.Values;
        public abstract BotType BotType { get; }
        public abstract Task SendMessage(string chatId, string message, string[][] keyboard = null, string[][] inlineKeyboard = null);
        protected virtual string[][] Keyboard => botManager.GetKeyboard(this);
        public virtual Dictionary<MarkType, string> Marks => new Dictionary<MarkType, string>() 
        {
            [MarkType.Stop] = "🚫",          //🛇
            [MarkType.CheckLight] = "✅",    //🗸
        };

        public async Task SendBroadCastMessage(string chatId, string message, string[][] keyboard = null, string[][] inlineKeyboard = null)
        {
            await SendMessage(chatId, message, keyboard, inlineKeyboard);

            foreach (var chat in chats.Values.Where(c => c.IsGroup && c.ChatId != chatId))
                await SendMessage(chat.ChatId, message, keyboard, inlineKeyboard);
        }

        public async Task SendBroadCastMessage(string message, string[][] keyboard = null, string[][] inlineKeyboard = null)
        {
            foreach (var chat in chats.Values.Where(c => c.IsGroup))
                await SendMessage(chat.ChatId, message, keyboard, inlineKeyboard);
        }

        public virtual async Task Start(BotManager botManager, CancellationToken cancellationToken)
        {
            this.cancellationToken = cancellationToken;
            this.botManager = botManager;

            Debug.WriteLine($"INFO: {BotType} Chats {chats.ToJsonStr()}");
        }

        protected async Task CommandAction(string chatId, string actionName)
        {
            var action = botManager.GetAction(this, actionName);

            if (action != null)
            {
                if (chats.TryGetValue(chatId, out var chat))
                    await action(this, chat, actionName, cancellationToken);
                else
                    await SendMessage(chatId, "⚠️ Выполните подключение бота к этому чату командой '/start'");
            }
        }

        protected async Task CommandStart(BotStartArgs args)
        {
            var chatId = args.ChatId;
            var isGroupChat = args.IsGroupChat;
            
            if (!chats.TryGetValue(chatId, out var chat))
            {
                chat = new Chat()
                {
                    ChatId = chatId.ToString(),
                    BotType = BotType,
                    IsGroup = isGroupChat,
                    UserName = args.UserName,
                    Title = args.ChatTitle,
                };

                chats.Add(chatId, chat);
            }

            var chatCount = chats.Values.Count(c => c.BotType == BotType);
            var groupChatCount = chats.Values.Count(c => c.IsGroup && c.BotType == BotType);

            if (args.CommandArgs.Length <= 1)
            {
                await SendMessage(args.ChatId, $"👋 Ваш АТС бот", Keyboard);
            }
        }

        protected BotBase(
            IServiceProvider sp
            )
        {
            this.sp = sp;
            this.clientOptions = sp.GetRequiredService<IOptions<ClientOptions>>().Value;
            this.configuration = sp.GetRequiredService<IConfiguration>();
        }
    }
}
