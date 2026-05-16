using Calls.Api.Bots;
using Calls.Entities.Json;
using Calls.HttpClients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Model.Enums;
using Shared.Model.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private IConfiguration configuration;
        private Task[] startBotTasks;
        private ClientData data;
        
        public DateTime StartTime = DateTime.Now;

        public event Action<string> OnIntegrationMessageResult;

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

        public async Task SchedulePolling()
        {
            while (true) 
            { 
                if (data.ScheduleIntervalMinutes > 0)
                {
                    var dayMinutes = (int)DateTime.Now.TimeOfDay.TotalMinutes;

                    if (dayMinutes % data.ScheduleIntervalMinutes == 0 && bot != null)
                    {
                        Debug.WriteLine($"{DateTime.Now} report message");
                        var htmlMessage = await callsManager.GetUniqueCallsMessage(false, true, true, cancellationTokenSource.Token);
                        await bot.SendBroadCastMessage(htmlMessage[0]);

                        if (data.IntegrationData.HasIntegration)
                        {
                            var integrationMessage = new IntegrationMessage
                            {
                                IsHtml = data.IntegrationData.IsHtml,
                                Message = htmlMessage[0]
                            };

                            if (!data.IntegrationData.IsHtml)
                            {
                                var textMessage = await callsManager.GetUniqueCallsMessage(false, false, true, cancellationTokenSource.Token);
                                integrationMessage.Message = textMessage[0];
                            }

                            var result = await integrationHttpClient.SendMessage(integrationMessage, cancellationTokenSource.Token);
                            OnIntegrationMessageResult.Raise(result);
                        }
                    }
                }

                var delay = 1000 * (60 - (((int)DateTime.Now.TimeOfDay.TotalSeconds) % 60));
                await Task.Delay(delay);
            }
        }

        public async Task Restart()
        {
            this.configuration = sp.GetRequiredService<IConfiguration>();
            this.callsManager = sp.GetRequiredService<CallsManager>();

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            bot = new RemoteBot(sp);
            await bot.Start(this, cancellationTokenSource.Token);
        }
    }
}
