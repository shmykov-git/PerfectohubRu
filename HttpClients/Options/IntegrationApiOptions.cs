using Shared.HttpClients.Options.Base;

namespace Calls.HttpClients.Options
{
    public class IntegrationApiOptions : HttpClientOptions
    {
        public MethodOptions SendMessage { get; set; }
    }
}
