using Calls.HttpClients.Options;
using MapsterMapper;
using Microsoft.Extensions.Options;
using PerfectohubRu.Model;
using PerfectohubRu.Tools;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public class Tele2AtsTestHttpClient : Tele2AtsHttpClient
    {
        private readonly ClientData data;

        public Tele2AtsTestHttpClient(
            HttpClient client, 
            IOptions<Tele2AtsApiOptions> options, 
            ClientDataProvider dataProvider, 
            IMapper mapper, 
            IServiceProvider sp) : base(client, options, dataProvider, mapper, sp)
        {
            data = dataProvider.Data;
        }

        public override async Task<(string accessToken, string refreshToken)> GetRefreshToken(string refreshToken)
        {
            return (data.Tele2AtsToken, refreshToken);
        }
    }
}
