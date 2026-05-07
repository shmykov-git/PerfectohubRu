using Calls.HttpClients.Abstractions;
using Calls.HttpClients.Base;
using Calls.HttpClients.Options;
using Calls.Model.Ats;
using Calls.Model.Ats.Tele2;
using MapsterMapper;
using Microsoft.Extensions.Options;
using PerfectohubRu.Tools;
using Shared.Exceptions.Cases;
using Shared.Extensions;
using Shared.HttpClients.Base;
using Shared.HttpClients.Options.Base;
using Shared.Model.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public class Tele2AtsHttpClient : AtsHttpClientBase<AtsTele2Call>, ITele2AtsHttpClient
    {
        private readonly Tele2AtsApiOptions options;
        private readonly ClientDataProvider clientStateProvider;
        private readonly IMapper mapper;

        public Tele2AtsHttpClient(
            HttpClient client, 
            IOptions<Tele2AtsApiOptions> options,
            ClientDataProvider clientStateProvider,
            IMapper mapper, 
            IServiceProvider sp
            ) : base(client, sp, options)
        {
            this.options = options.Value;
            this.clientStateProvider = clientStateProvider;
            this.mapper = mapper;
        }

        public override AtsType AtsType => AtsType.Tele2;

        public async Task<AtsTele2Abonent[]> GetTele2Abonents()
        {
            var token = clientStateProvider.Data.Tele2AtsToken;

            return await GetAsync<AtsTele2Abonent[]>(new MethodArgs
            {
                Method = options.GetAbonents,
                QueryArgsType = QueryArgsType.QueryString,
                GetAuthorizationToken = () => token,
                UseThrowCase = HttpClientCase.Tele2Ats
            });
        }

        public async Task<ActivePhone[]> GetAbonents()
        {
            var beelineAbonents = await GetTele2Abonents();
            var abonents = beelineAbonents.Select(mapper.Map<ActivePhone>).ToArray();

            return abonents;
        }

        protected override bool FilterAtsCalls(AtsTele2Call callItem) => true;

        public async Task<AtsCall[]> GetCalls(DateTime fromUtc, DateTime toUtc)
        {
            var tele2Calls = await GetTele2Calls(fromUtc, toUtc);
            var calls = tele2Calls.Select(mapper.Map<AtsCall>).ToArray();

            return calls;
        }

        public async Task<AtsPassportCall[]> GetPassportCalls(DateTime fromUtc, DateTime toUtc)
        {
            var tele2Calls = await GetTele2Calls(fromUtc, toUtc);
            var passportCalls = tele2Calls.Select(mapper.Map<AtsPassportCall>).ToArray();

            return passportCalls;
        }

        public Task<AtsTele2Call[]> GetTele2Calls(DateTime fromUtc, DateTime toUtc) => GetHttpDayCalls(fromUtc.Date.KindOfUtc());

        protected override async Task<AtsTele2Call[]> GetHttpDayCalls(DateTime todayUtc)
        {
            Debug.WriteLine($"{ServiceName} request {todayUtc.ToString("yyyy.MM.dd")} data");

            var token = clientStateProvider.Data.Tele2AtsToken;

            var query = new
            {
                start = new DateTimeOffset(todayUtc).ToString("o"),
                end = new DateTimeOffset(todayUtc.AddDays(1)).ToString("o"),
                size = options.Limit, // max = 2000
                page = 0
            };

            var calls = await GetAsync<AtsTele2Call[]>(query, new MethodArgs
            {
                Method = options.GetCalls,
                QueryArgsType = QueryArgsType.QueryString,
                GetAuthorizationToken = () => token,
                UseThrowCase = HttpClientCase.Tele2Ats
            });

            return calls;
        }

        public async Task<(string accessToken, string refreshToken)> GetRefreshToken(string refreshToken)
        {
            var token = await PutAsync<AtsTele2RefreshToken>(new { }, new MethodArgs
            {
                Method = options.RefreshToken,
                QueryArgsType = QueryArgsType.JsonBody,
                GetAuthorizationToken = () => refreshToken,
                UseThrowCase = HttpClientCase.NoAuthToken
            });

            return (token.AccessToken, token.RefreshToken);
        }
    }
}
