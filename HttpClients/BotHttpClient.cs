using Calls.HttpClients.Options;
using Calls.Model.Ats.Beeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Exceptions.Cases;
using Shared.HttpClients.Base;
using Shared.HttpClients.Options.Base;
using Shared.Model.Enums;
using Shared.Model.Options;
using System;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public class BotHttpClient : HttpClientBase, IBotHttpClient
    {
        private readonly BotsApiOptions options;
        private readonly ClientOptions clientOptions;

        public BotHttpClient(HttpClient client, IOptions<ClientOptions> clientOptions, IServiceProvider sp, IOptions<BotsApiOptions> options) : base(client, sp, options)
        {
            this.options = options.Value;
            this.clientOptions = clientOptions.Value;
        }

        public async Task<RegisterResult> Register(BotType type, string botToken, string oldBotId)
        {
            var query = new
            {
                type,
                token = botToken,
                botId = oldBotId
            };

            var result = await PostAsync<RegisterResult>(query, new MethodArgs
            {
                Method = options.Register,
                QueryArgsType = QueryArgsType.JsonBody,
                GetBasicAuth = () => (clientOptions.BotAuth.Username, clientOptions.BotAuth.Password),
                UseThrowCase = HttpClientCase.Bots
            });

            return result;
        }

        public async Task<MessageOut[]> GetOutMessages(string botId)
        {
            var result = await GetAsync<MessageOut[]>(new MethodArgs
            {
                Method = options.GetOutMessages.With(("botId", botId)),
                QueryArgsType = QueryArgsType.QueryString,
                GetBasicAuth = () => (clientOptions.BotAuth.Username, clientOptions.BotAuth.Password),
                UseThrowCase = HttpClientCase.Bots
            });

            return result;
        }

        public async Task<RegisterResult> SendMessage(string botId, string chatId, MessageIn message)
        {
            var result = await PostAsync<RegisterResult>(message, new MethodArgs
            {
                Method = options.SendChatMessage.With(("botId", botId), ("chatId", chatId)),
                QueryArgsType = QueryArgsType.JsonBody,
                GetBasicAuth = () => (clientOptions.BotAuth.Username, clientOptions.BotAuth.Password),
                UseThrowCase = HttpClientCase.Bots
            });

            return result;
        }
    }
}
