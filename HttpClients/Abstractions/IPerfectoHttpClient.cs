using PerfectohubRu.Model;
using System.Threading.Tasks;

namespace Calls.HttpClients
{
    public interface IPerfectoHttpClient
    {
        Task<bool> IsClientIdAvailable();
        Task<ServerOperatonResult> RunOnServer();
        Task<ServerOperatonResult> StopOnServer();
    }
}
