using Calls.Bot.Services;
using Calls.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Model.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Calls.Api.Bots
{
    public class RemoteBot : BotBase
    {
        private IBotHttpClient httpClient;
        private ClientDataProvider dataProvider;
        private ClientData data;

        public override BotType BotType => data.BotType;

        public RemoteBot(IServiceProvider sp) : base(sp)
        {
            this.httpClient = sp.GetRequiredService<IBotHttpClient>();
            this.dataProvider = sp.GetRequiredService<ClientDataProvider>();
            this.data = dataProvider.Data;
        }

        public override async Task Start(BotManager manager, CancellationToken cancellationToken)
        {
            try
            {
                await base.Start(manager, cancellationToken);
                var registredBotId = data.BotId;
                var result = await httpClient.Register(data.BotType, data.BotToken, registredBotId);

                if (!result.Success)
                {
                    Debug.WriteLine($"ERROR: Cannot register {data.BotType} {data.BotToken}");

                    return;
                }

                data.BotId = result.Id;
                dataProvider.Save();
                IsStarted = true;

                _ = Task.Run(() => Polling(cancellationToken));

                if (registredBotId != null && data.BotId != registredBotId)
                    Debug.WriteLine($"INFO: {BotType} @{registredBotId} старый бот остановлен");

                Debug.WriteLine($"INFO: {BotType} @{data.BotId} запущен и готов к работе!");
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"ERROR: {BotType} @{data.BotId} не удалось запустить. Error: {ex.Message}");
            }
        }

        public async Task Polling(CancellationToken token)
        {
            while (!token.IsCancellationRequested && IsStarted)
            {
                try
                {
                    if (data.IsLocalRun)
                    {
                        var messages = await httpClient.GetOutMessages(data.BotId);

                        DispatcherHelper.Dispatch(async () =>
                        {
                            foreach (var message in messages)
                                await BotOnMessage(message);
                        });
                    }
                }
                catch (Exception ex)
                { 
                    Debug.WriteLine($"~Polling error: {ex.Message}");
                }

                await Task.Delay(clientOptions.BotPollingInterval);
            }
        }

        private async Task BotOnMessage(MessageOut message)
        {
            var chatId = message.ChatId;
            var isGroupChat = message.IsGroupChat;
            var chatTitle = message.ChatTitle;
            var userName = message.UserName;
            var messageText = message.Text;

            Debug.WriteLine($"DEBUG: {chatId} {userName}: '{messageText}'");

            if (messageText.StartsWith("/"))
            {
                var commandArgs = messageText.Split(' ').ToArray();

                switch (commandArgs[0].ToLower())
                {
                    case "/start":
                        await CommandStart(new BotStartArgs 
                        { 
                            ChatId = chatId.ToString(),
                            IsGroupChat = isGroupChat,
                            CommandArgs = commandArgs,                        
                            UserName = userName,
                            ChatTitle = chatTitle,
                        });
                        break;
 
                    default:
                        await SendMessage(chatId.ToString(), "⚠️ Используйте команду /start");
                        break;
                }
            }
            else
            {
                await CommandAction(chatId.ToString(), messageText);
            }
        }

        public override async Task SendMessage(string chatId, string message, string[][] keyboard = null, string[][] inlineKeyboard = null)
        {
            if (!IsStarted)
            {
                Debug.WriteLine($"WARN: {BotType} is not started yet");

                return;
            }

            try
            {
                await httpClient.SendMessage(data.BotId, chatId, new MessageIn
                {
                    Text = message,
                    Keyboard = keyboard,
                    InlineKeyboard = inlineKeyboard
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: Cannot send message '{ex.ToString()}'");
            }
        }

    }
}
