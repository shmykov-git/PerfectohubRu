using Calls.Model.Ats.Beeline;
using Calls.Model.Ats.Tele2;
using System;
using System.Threading.Tasks;

namespace Calls.HttpClients.Abstractions
{
    public interface ITele2AtsHttpClient : IAtsHttpClient
    {
        Task<AtsTele2Call[]> GetTele2Calls(DateTime from, DateTime to);
        Task<AtsTele2Abonent[]> GetTele2Abonents();
    }
}