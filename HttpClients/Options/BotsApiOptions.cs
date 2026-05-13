using Shared.HttpClients.Options.Base;

namespace Calls.HttpClients.Options
{
    public class BotsApiOptions : HttpClientOptions
    {
        public MethodOptions Register { get; set; }
        public MethodOptions GetOutMessages { get; set; }
        public MethodOptions SendChatMessage { get; set; }
    }
}
