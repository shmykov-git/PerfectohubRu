using Calls.Bot.Services;
using Calls.HttpClients;
using Calls.HttpClients.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Perfecto.Deploy.Extensions;
using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Exceptions;
using Shared.Extensions;
using Shared.HttpClients.Base;
using Shared.Libraries;
using Shared.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PerfectohubRu.Forms.ViewModles
{
    public partial class MainViewModel
    {
        private readonly IAtsHttpClient httpClient;
        private readonly BotManager botManager;
        private readonly CallsManager callsManager;
        private readonly ClientDataProvider dataProvider;
        private readonly RefreshTokenManager refreshTokenManager;
        private readonly IPerfectoHttpClient perfectoHttpClient;
        private readonly IServiceProvider sp;
        private ClientData data;

        public MainViewModel(
            BotManager botManager,
            CallsManager callsManager,
            ClientDataProvider clientDataProvider,
            RefreshTokenManager refreshTokenManager,
            IPerfectoHttpClient perfectoHttpClient,
            IServiceProvider sp)
        {
            this.botManager = botManager;
            this.callsManager = callsManager;
            this.dataProvider = clientDataProvider;
            this.refreshTokenManager = refreshTokenManager;
            this.perfectoHttpClient = perfectoHttpClient;
            this.sp = sp;
            data = clientDataProvider.Data;
            AtsToken = data.GetAtsToken();
            BotToken = data.BotToken;
            Knowns = data.Knowns.SJoin("\r\n");
            Commons = data.Commons.SJoin("\r\n");

            ScheduleItems = new ScheduleItem[]
            {
                new ScheduleItem { Name = "Каждую минуту", IntervalMinutes = 1 },
                new ScheduleItem { Name = "Каждые 5 минут", IntervalMinutes = 5 },
                new ScheduleItem { Name = "Каждые 10 минут", IntervalMinutes = 10 },
                new ScheduleItem { Name = "Каждые 15 минут", IntervalMinutes = 15 },
                new ScheduleItem { Name = "Каждые 30 минут", IntervalMinutes = 30 },
                new ScheduleItem { Name = "Каждый час", IntervalMinutes = 60 },
                new ScheduleItem { Name = "Каждые 2 часа", IntervalMinutes = 120 },
            };

            SelectedSchedule = ScheduleItems.FirstOrDefault(i => i.IntervalMinutes == data.ScheduleIntervalMinutes) ?? ScheduleItems[0];

            if (data.BotToken != null)
                _ = Task.Run(botManager.Restart);

            botManager.OnIntegrationMessage += value => IntegrationMessage = value;
            botManager.OnIntegrationMessageResult += 
                (value, color) => 
                { 
                    IntegrationMessageResult = value; 
                    IntegrationMessageResultColor = color;
                };

            refreshTokenManager.OnAtsTokenRefresh += () =>
            {
                OnPropertyChanged(nameof(AtsToken));
                OnPropertyChanged(nameof(AtsRefreshToken));
            };
        }

        public ClientData Data => data;

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

        public async void RefreshIntegrationMessage()
        {
            await botManager.RefreshIntegrationMessage();
        }

        private Task<string[]> GetCallsMessage() => callsManager.GetUniqueCallsMessage(false, false, false, default);

        public void ApproveKnowns()
        {
            var phones = Knowns.Replace("\r", "").Split('\n')
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.ToCorrectSystemPhone())
                .ToArray();

            Data.Knowns = phones;
            dataProvider.Save();
            Knowns = phones.SJoin("\r\n");
            RefreshCallsMessage();
        }

        public void ApproveCommons()
        {
            var phones = Commons.Replace("\r", "").Split('\n')
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Select(v => v.ToCorrectSystemPhone())
                .ToArray();

            Data.Commons = phones;
            dataProvider.Save();
            Commons = phones.SJoin("\r\n");
            RefreshCallsMessage();
        }

        public async Task ApproveMessage()
        {
            State = ClientState.HasMessage;
        }

        public async Task ApproveBot()
        {
            State = ClientState.HasTestedBot;
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

                dataProvider.Save();

                State = ClientState.HasAts;

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
                return new OperationResult { Error = "Не удалось определить тип токена чат-бота" };

            try
            {
                data.BotToken = token;
                data.BotType = botType;
                State = ClientState.HasBot;

                await botManager.Restart();

                return OperationResult.Successfull();
            }
            catch (HttpClientException)
            {
                return new OperationResult { Error = $"Не удается запустить чат-бота" };
            }
        }

        public async Task SaveSchedule(ScheduleItem item)
        {
            data.ScheduleIntervalMinutes = item.IntervalMinutes;
            dataProvider.Save();
        }

        public async Task ApproveIntegration()
        {
            State = ClientState.HasIntegration;
        }

        public async void ValidateClientId()
        {
            await ReactHelper.LastActionIn(nameof(ValidateClientId), TimeSpan.FromSeconds(2));

            if (!IsValidClientId())
            {
                ServerResultMessageColor = Brushes.Red;
                ServerResultMessage = "Невалидный идентификатор";
                return;
            }

            if (!await IsAvailableClientId())
            {
                ServerResultMessageColor = Brushes.Red;
                ServerResultMessage = "Идентификатор используется";
                return;
            }

            ServerResultMessageColor = Brushes.Green;
            ServerResultMessage = "Доступно";
        }

        public async void ValidatePassword()
        {
            await ReactHelper.LastActionIn(nameof(ValidatePassword), TimeSpan.FromSeconds(2));

            if (!IsValidClientPassword())
            {
                ServerResultMessageColor = Brushes.Red;
                ServerResultMessage = "Слишком короткий";
                return;
            }

            ServerResultMessageColor = Brushes.Green;
            ServerResultMessage = "Пойдет";
        }

        public bool IsValidClientId() => ClientId != null && Values.Regexes.ClientId.IsMatch(ClientId);
        public Task<bool> IsAvailableClientId() => perfectoHttpClient.IsClientIdAvailable();

        public bool IsValidClientPassword() => ClientPassword.Length >= 4;

        public async Task RunOnServer()
        {
            
            //IsServerRun = true;
        }

        public async Task StopOnServer()
        {

            //IsServerRun = false;
        }
    }
}
