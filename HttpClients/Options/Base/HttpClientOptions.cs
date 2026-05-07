namespace Shared.HttpClients.Options.Base
{
    public class HttpClientOptions
    {
        public string Host { get; set; }
        public int? ClientTimeout { get; set; }
        public int? RetryCount { get; set; }
        public int? RetryTimeout { get; set; }
        public int? DebugContentLimit { get; set; }
        public bool? ServerCertificateCustomValidationCallbackReturnTrue { get; set; }
    }
}
