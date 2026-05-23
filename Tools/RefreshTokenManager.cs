using Calls.HttpClients.Abstractions;
using Microsoft.Extensions.Options;
using PerfectohubRu.Extensions;
using PerfectohubRu.Model;
using Shared.Model.Enums;
using Shared.Model.Options;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace PerfectohubRu.Tools
{
    public class RefreshTokenManager
    {
        private readonly IAtsHttpClient atsHttpClient;
        private readonly ClientOptions clientOptions;
        private readonly ClientDataProvider dataProvider;
        private readonly ClientData data;

        public event Action OnAtsTokenRefresh;

        public RefreshTokenManager(
            IAtsHttpClient atsHttpClient, 
            IOptions<ClientOptions> clientOptions, 
            ClientDataProvider dataProvider)
        {
            this.atsHttpClient = atsHttpClient;
            this.clientOptions = clientOptions.Value;
            this.dataProvider = dataProvider;
            this.data = dataProvider.Data;

            _ = Task.Run(async () => RefreshTokenPolling(default));
        }

        private async Task RefreshTokenPolling(CancellationToken token)
        {
            if (clientOptions.RefreshTokenPollingHoursInterval > 24 || clientOptions.RefreshTokenPollingHoursInterval <= 0)
                throw new ArgumentException("RefreshTokenPollingHoursInterval must be in (0, 24]");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var dayHours = (int)DateTime.Now.TimeOfDay.TotalHours;
                    var delay = 1000 * 60 * 60 * (24 - (((int)DateTime.Now.TimeOfDay.TotalHours) % 24));
                    await Task.Delay(delay);

                    if ((int)DateTime.Now.TimeOfDay.TotalHours % clientOptions.RefreshTokenPollingHoursInterval != 0)
                        continue;

                    if (data.IsServerRun)
                        continue;

                    if (data.GetAtsType() == AtsType.Tele2)
                    {
                        if (IsValidTele2RefreshToken(data.Tele2AtsRefreshToken))
                        {
                            var (accessToken, refreshToken) = await atsHttpClient.GetRefreshToken(data.Tele2AtsRefreshToken);
                            data.Tele2AtsToken = accessToken;
                            data.Tele2AtsRefreshToken = refreshToken;
                            dataProvider.Save();
                            DispatcherHelper.Dispatch(OnAtsTokenRefresh.Raise);
                        }
                        else
                            Debug.WriteLine($"WARN: invalid ats refresh token '{data.Tele2AtsRefreshToken}'");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ERROR: ~Polling refresh error: {ex.Message}");
                }
            }
        }

        private bool IsValidTele2RefreshToken(string token)
        {
            var dotsCount = token.Length - token.Replace(".", "").Length;
            return dotsCount >= 3;
        }
    }
}
