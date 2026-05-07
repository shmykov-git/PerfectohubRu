using Calls.Model.Ats.Beeline;
using System;
using System.Threading.Tasks;

namespace Calls.HttpClients.Abstractions
{
    public interface IBeelineAtsHttpClient : IAtsHttpClient
    {
        Task<AtsBeelineView.AtsBeelineCall[]> GetBeelineCalls(DateTime from, DateTime to);
        Task<AtsBeelineAbonent[]> GetBeelineAbonents();
    }
}