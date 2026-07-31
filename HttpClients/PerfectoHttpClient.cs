using Calls.Entities.Json;
using Calls.HttpClients.Options;
using Calls.Model.Ats.Beeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using Shared.Exceptions;
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
            var query = new
            {
                userName = data.ClientId
            };

            var result = await GetAsync<bool>(query, new MethodArgs
            {
                Method = options.IsClientIdAvailable,
                QueryArgsType = QueryArgsType.QueryString
            });

            return result;
        }

        public async Task<ServerOperatonResult> RunOnServer()
        {
            try
            {
                var result = await PostAsync<ServerOperatonResult>(data, new MethodArgs
                {
                    Method = options.RunOnServer,
                    QueryArgsType = QueryArgsType.JsonBody,
                    UseThrowCase = HttpClientCase.PerfectoApi
                });

                return result;
            }
            catch (HttpClientException ex) when (ex.Case == HttpClientCase.PerfectoApi)
            {
                return new ServerOperatonResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
            catch (Exception ex) 
            { 
                return new ServerOperatonResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<ServerOperatonResult> StopOnServer()
        {
            try
            {
                var query = new
                {
                    clientId = data.ClientId,
                    clientPassword = data.ClientPassword,
                    clientMac = data.ClientMac,
                };

                var result = await PostAsync<ServerOperatonResult>(query, new MethodArgs
                {
                    Method = options.StopOnServer,
                    QueryArgsType = QueryArgsType.JsonBody,
                    UseThrowCase = HttpClientCase.PerfectoApi
                });

                return result;
            }
            catch (HttpClientException ex) when (ex.Case == HttpClientCase.PerfectoApi)
            {
                return new ServerOperatonResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new ServerOperatonResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
    }
}
