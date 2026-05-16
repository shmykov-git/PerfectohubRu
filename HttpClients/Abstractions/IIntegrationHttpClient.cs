using Calls.Entities.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public interface IIntegrationHttpClient
    {
        Task<string> SendMessage(IntegrationMessage message, CancellationToken token);
    }
}
