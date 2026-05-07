using Calls.Model.Ats;
using Calls.Model.Ats.Beeline;
using System;
using System.Threading.Tasks;

namespace Calls.HttpClients.Abstractions
{
    public interface IAtsHttpClient
    {
        Task<AtsCall[]> GetCalls(DateTime fromUtc, DateTime toUtc);
        Task<ActivePhone[]> GetAbonents();
        Task<AtsPassportCall[]> GetPassportCalls(DateTime fromUtc, DateTime toUtc);

        Task<(string accessToken, string refreshToken)> GetRefreshToken(string refreshToken);
    }
}
