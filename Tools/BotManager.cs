using Calls.Api.Bots;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
        private RemoteBot bot = null;
        private IConfiguration configuration;
        private Task[] startBotTasks;
        private ClientData data;
        
        public DateTime StartTime = DateTime.Now;

        public BotManager(
            IServiceProvider sp,
            CallsManager callsManager,
            IOptions<ClientOptions> clientOptions
            )
        {
            this.sp = sp;
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
                        var message = await callsManager.GetUniqueCallsMessage(false, true, true, cancellationTokenSource.Token);
                        await bot.SendBroadCastMessage(message[0]);
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
