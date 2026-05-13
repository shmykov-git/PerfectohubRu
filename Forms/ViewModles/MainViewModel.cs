using Calls.Bot.Services;
using Calls.HttpClients.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Perfecto.Deploy.Extensions;
using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Exceptions;
using Shared.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectohubRu.Forms.ViewModles
{
    public partial class MainViewModel
    {
        private readonly IAtsHttpClient httpClient;
        private readonly BotManager botManager;
        private readonly CallsManager callsManager;
        private readonly ClientDataProvider dataProvider;
        private readonly IServiceProvider sp;
        private ClientData data;

        public MainViewModel(
            BotManager botManager,
            CallsManager callsManager,
            ClientDataProvider clientDataProvider,
            IServiceProvider sp)
        {
            this.botManager = botManager;
            this.callsManager = callsManager;
            this.dataProvider = clientDataProvider;
            this.sp = sp;
            data = clientDataProvider.Data;
            AtsToken = data.GetAtsToken();
            BotToken = data.BotToken;
            Knowns = data.Knowns.SJoin("\r\n");
            Commons = data.Commons.SJoin("\r\n");

            if (data.BotToken != null)
                _ = Task.Run(botManager.Restart);
        }

        public ClientData ClientData => data;
        public IServiceProvider Sp => sp;

        private AtsType GetTokenAtsType(string token)
        {
            var dotsCount = token.Length - token.Replace(".", "").Length;
            //var munisesCount = token.Length - token.Replace("-", "").Length;

            if (dotsCount >= 3)
                return AtsType.Tele2;

            return AtsType.Beeline;
        }

        private BotType GetBotTokenType(string token)
        {
            if (token == null || token.Length <= 11)
                return BotType.Unknown;

            if (token[10] == ':' && token.Length > 40 && token.Length < 80)
                return BotType.Telegram;

            if (token.Length > 80 && token.Length < 200)
                return BotType.Max;

            return BotType.Unknown;
        }

        public async void RefreshCallsMessage()
        {
            await Task.Run(async () =>
            {
                try
                {
                    CallsMessage = (await GetCallsMessage()).First();
                }
                catch (Exception ex)
                {
                    data.CriticalError = ex.Message;
                    dataProvider.Save();
                }
            });
        }

        private Task<string[]> GetCallsMessage() => callsManager.GetUniqueCallsMessage(false, false, false, default);

        public void SaveKnowns()
        {
            var phones = Knowns.Replace("\r", "").Split('\n').Select(v => v.Trim()).ToArray();
            ClientData.Knowns = phones;
            dataProvider.Save();
            RefreshCallsMessage();
        }

        public void SaveCommons()
        {
            var phones = Commons.Replace("\r", "").Split('\n').Select(v => v.Trim()).ToArray();
            ClientData.Commons = phones;
            dataProvider.Save();
            RefreshCallsMessage();
        }

        public async Task ApproveMessage()
        {
            data.State = ClientState.HasMessage;
            dataProvider.Save();
        }

        public async Task<OperationResult> ValidateAndSaveAtsToken()
        {
            var token = AtsToken;
            var atsType = GetTokenAtsType(token);

            switch (atsType) 
            {
                case AtsType.Beeline:
                    data.BeelineAtsToken = token;
                    break;

                case AtsType.Tele2:
                    data.Tele2AtsToken = token;
                    break;

                default:
                    return new OperationResult { Error = "Не удалось определить тип токена" };
            }

            dataProvider.Save();

            var httpClient = sp.GetRequiredService<IAtsHttpClient>();

            try
            {
                var actives = await httpClient.GetAbonents();
                data.Actives = actives;

                if (actives.Length == 0)
                    return new OperationResult { Error = $"Требуется настроить список абонентов в АТС" };

                data.State = ClientState.HasAts;
                dataProvider.Save();

                return OperationResult.Successfull();
            }
            catch (HttpClientException)
            {
                return new OperationResult { Error = $"Не удается установить подключение к {data.GetAtsName()}" };
            }
        }

        public async Task<OperationResult> ValidateAndSaveBotToken()
        {
            var token = BotToken;
            var botType = GetBotTokenType(token);
            
            if (botType == BotType.Unknown)
                return new OperationResult { Error = "Не удалось определить тип токена чат бота" };

            try
            {
                data.BotToken = token;
                data.BotType = botType;
                data.State = ClientState.HasBot;
                dataProvider.Save();

                await botManager.Restart();

                return OperationResult.Successfull();
            }
            catch (HttpClientException)
            {
                return new OperationResult { Error = $"Не удается запустить бота" };
            }
        }
    }
}
