using Shared.Model.Enums;

namespace Shared.HttpClients.Options.Base
{
    public class AtsHttpClientOptions : HttpClientOptions
    {
        public MethodOptions GetCalls { get; set; }
        public MethodOptions GetAbonents { get; set; }
        public int Limit { get; set; }
        public bool UseCache { get; set; }
        public MethodOptions RefreshToken { get; set; }
        public int MaxParallelRequestsCount { get; set; }
    }
}