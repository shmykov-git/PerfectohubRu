using Calls.Entities.Json;
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
using System.Windows.Markup;

namespace Calls.HttpClients
{
    public class PerfectoHttpClient : HttpClientBase, IPerfectoHttpClient
    {
        private readonly PerfectoApiOptions options;
        private readonly ClientOptions clientOptions;
        private readonly ClientData data;

        public PerfectoHttpClient(HttpClient client, IOptions<ClientOptions> clientOptions, IServiceProvider sp, IOptions<PerfectoApiOptions> options) : base(client, sp, options)
        {
            this.options = options.Value;
            this.clientOptions = clientOptions.Value;
            this.data = sp.GetRequiredService<ClientDataProvider>().Data;
        }

        public async Task<bool> IsClientIdAvailable()
        {
            var result = await GetAsync<bool>(new MethodArgs
            {
                Method = options.IsClientIdAvailable.With(("username", data.ClientId)),
                QueryArgsType = QueryArgsType.QueryString,
                UseThrowCase = HttpClientCase.PerfectoApi
            });

            return result;
        }

        public async Task<ServerOperatonResult> RunOnServer()
        {
            var result = await PostAsync<ServerOperatonResult>(data, new MethodArgs
            {
                Method = options.RunOnServer,
                QueryArgsType = QueryArgsType.JsonBody,
                GetBasicAuth = () => (data.ClientId, data.ClientPassword),
                UseThrowCase = HttpClientCase.PerfectoApi
            });

            return result;
        }

        public async Task<ServerOperatonResult> StopOnServer()
        {
            var result = await PostAsync<ServerOperatonResult>(new MethodArgs
            {
                Method = options.RunOnServer,
                QueryArgsType = QueryArgsType.JsonBody,
                GetBasicAuth = () => (data.ClientId, data.ClientPassword),
                UseThrowCase = HttpClientCase.PerfectoApi
            });

            return result;
        }
    }
}
