using Shared.HttpClients.Options.Base;

namespace Calls.HttpClients.Options
{
    public class PerfectoApiOptions : HttpClientOptions
    {
        public MethodOptions IsClientIdAvailable { get; set; }
        public MethodOptions RunOnServer { get; set; }
        public MethodOptions StopOnServer { get; set; }
    }
}
