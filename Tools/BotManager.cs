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
            IOptions<ClientOptions> clientOptions
            )
        {
            this.sp = sp;
            this.clientOptions = clientOptions.Value;
        }

        public async Task Restart()
        {
            this.configuration = sp.GetRequiredService<IConfiguration>();
            this.callsManager = sp.GetRequiredService<CallsManager>();
            this.data = sp.GetRequiredService<ClientDataProvider>().Data;

            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            cancellationTokenSource = new CancellationTokenSource();

            bot = new RemoteBot(sp);
            await bot.Start(this, cancellationTokenSource.Token);
        }
    }
}
