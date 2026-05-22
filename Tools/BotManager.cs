using Calls.Api.Bots;
using Calls.Entities.Json;
using Calls.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Model.Options;
using System;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Calls.Bot.Services
{
    public partial class BotManager
    {
        private CancellationTokenSource cancellationTokenSource;
        private ClientOptions clientOptions;
        private CallsManager callsManager;
        private readonly IServiceProvider sp;
        private readonly IIntegrationHttpClient integrationHttpClient;
        private RemoteBot bot = null;
        private ClientData data;
        
        public DateTime StartTime = DateTime.Now;

        public event Action<string> OnIntegrationMessage;
        public event Action<string, Brush> OnIntegrationMessageResult;

        public BotManager(
            IServiceProvider sp,
            IIntegrationHttpClient integrationHttpClient,
            CallsManager callsManager,
            IOptions<ClientOptions> clientOptions
            )
        {
            this.sp = sp;
            this.integrationHttpClient = integrationHttpClient;
            this.callsManager = callsManager;
            this.clientOptions = clientOptions.Value;
            this.data = sp.GetRequiredService<ClientDataProvider>().Data;

            _ = Task.Run(SchedulePolling);
        }

        public async Task RefreshIntegrationMessage(string[] htmlMessage = null)
        {
            var integrationMessage = new IntegrationMessage
            {
                IsHtml = data.IntegrationData.IsHtml,
            };

            if (data.IntegrationData.IsHtml)
            {
                htmlMessage = htmlMessage ?? await callsManager.GetUniqueCallsMessage(false, true, true, cancellationTokenSource.Token);
                integrationMessage.Message = htmlMessage[0];
            }
            else
            {
                var textMessage = await callsManager.GetUniqueCallsMessage(false, false, true, cancellationTokenSource.Token);
                integrationMessage.Message = textMessage[0];
            }

            OnIntegrationMessage.Raise(integrationMessage.Message);

            if (!data.IntegrationData.HasIntegration)
            {
                OnIntegrationMessageResult.Raise("Не подключено", Brushes.Yellow);

                return;
            }

            try
            {
                var result = await integrationHttpClient.SendMessage(integrationMessage, cancellationTokenSource.Token);
                OnIntegrationMessageResult.Raise(result, new SolidColorBrush(Color.FromRgb(0xCC, 0xCC, 0xCC)));
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Error: {ex.Message}");
                OnIntegrationMessageResult.Raise(ex.Message, Brushes.Red);
            }
        }

        public async Task SchedulePolling()
        {
            while (true) 
            { 
                try
                {
                    if (data.ScheduleIntervalMinutes > 0)
                    {
                        var dayMinutes = (int)DateTime.Now.TimeOfDay.TotalMinutes;

                        if (dayMinutes % data.ScheduleIntervalMinutes == 0 && bot != null)
                        {
                            Debug.WriteLine($"{DateTime.Now} report message");
                            var htmlMessage = await callsManager.GetUniqueCallsMessage(false, true, true, cancellationTokenSource.Token);
                            await bot.SendBroadCastMessage(htmlMessage[0]);

                            DispatcherHelper.Dispatch(() => RefreshIntegrationMessage(htmlMessage));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }

                var delay = 1000 * (60 - (((int)DateTime.Now.TimeOfDay.TotalSeconds) % 60));
                await Task.Delay(delay);
            }
        }

        public async Task Restart()
        {
            this.callsManager = sp.GetRequiredService<CallsManager>();

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            bot = new RemoteBot(sp);
            await bot.Start(this, cancellationTokenSource.Token);
        }
    }
}
