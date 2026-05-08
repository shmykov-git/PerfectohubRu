using Calls.HttpClients.Abstractions;
using Calls.HttpClients.Base;
using Calls.HttpClients.Options;
using Calls.Model.Ats;
using Calls.Model.Ats.Beeline;
using MapsterMapper;
using Microsoft.Extensions.Options;
using PerfectohubRu.Tools;
using Shared.Exceptions.Cases;
using Shared.Extensions;
using Shared.HttpClients.Options.Base;
using Shared.Model.Enums;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public class BeelineAtsHttpClient : AtsHttpClientBase<AtsBeelineView.AtsBeelineCall>, IBeelineAtsHttpClient
    {
        private readonly BeelineAtsApiOptions options;
        private readonly IMapper mapper;
        private readonly ClientDataProvider clientStateProvider;

        public BeelineAtsHttpClient(
            HttpClient client, 
            IMapper mapper,
            IOptions<BeelineAtsApiOptions> options,
            ClientDataProvider clientStateProvider,
            IServiceProvider sp
            ) : base(client, sp, options)
        {
            this.options = options.Value;
            this.mapper = mapper;
            this.clientStateProvider = clientStateProvider;
        }

        public override AtsType AtsType => AtsType.Beeline;

        public async Task<AtsBeelineAbonent[]> GetBeelineAbonents()
        {
            var token = clientStateProvider.Data.BeelineAtsToken;

            return await GetAsync<AtsBeelineAbonent[]>(new MethodArgs
            {
                Method = options.GetAbonents,
                QueryArgsType = QueryArgsType.QueryString,
                GetBeelineAtsToken = () => token,
                UseThrowCase = HttpClientCase.BeelineAts
            });
        }

        public async Task<ActivePhone[]> GetAbonents()
        {
            var beelineAbonents = await GetBeelineAbonents();
            var abonents = beelineAbonents.Select(mapper.Map<ActivePhone>).ToArray();

            return abonents;
        }

        protected override bool FilterAtsCalls(AtsBeelineView.AtsBeelineCall callItem) => true;

        public async Task<AtsCall[]> GetCalls(DateTime fromUtc, DateTime toUtc)
        {
            var beelineCalls = await GetBeelineCalls(fromUtc, toUtc);
            var calls = beelineCalls.Select(mapper.Map<AtsCall>).ToArray();

            return calls;
        }

        public async Task<AtsPassportCall[]> GetPassportCalls(DateTime fromUtc, DateTime toUtc)
        {
            var beelineCalls = await GetBeelineCalls(fromUtc, toUtc);
            var passportCalls = beelineCalls.Select(mapper.Map<AtsPassportCall>).ToArray();

            return passportCalls;
        }

        public Task<AtsBeelineView.AtsBeelineCall[]> GetBeelineCalls(DateTime fromUtc, DateTime toUtc) => GetHttpDayCalls(fromUtc);

        protected override async Task<AtsBeelineView.AtsBeelineCall[]> GetHttpDayCalls(DateTime todayUtc)
        {
            // todo: logger.LogDebug($"{ServiceName} request {todayUtc.ToString("yyyy.MM.dd")} data");
            Debug.WriteLine($"{ServiceName} request {todayUtc.ToString("yyyy.MM.dd")} data");

            var token = clientStateProvider.Data.BeelineAtsToken;

            var query = new
            {
                from = todayUtc.ToIso8601(),
                to = todayUtc.AddDays(1).ToIso8601(),
                limit = options.Limit,
            };

            var view = await GetAsync<AtsBeelineView>(query, new MethodArgs
            {
                Method = options.GetCalls,
                QueryArgsType = QueryArgsType.QueryString,
                GetBeelineAtsToken = () => token,
                UseThrowCase = HttpClientCase.BeelineAts
            });

            return view.payload.calls;
        }

        public async Task<(string accessToken, string refreshToken)> GetRefreshToken(string refreshToken)
        {
            throw new NotSupportedException("Refresh Beeline Ats token is not supported");
        }
    }
}
