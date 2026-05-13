using Shared.Exceptions.Cases;
using System;
using System.Threading;

namespace Shared.HttpClients.Options.Base
{
    public class MethodOptions
    {
        public string MethodApi { get; set; }
        public int? RequestTimeout { get; set; }
        public int? RetryCount { get; set; }
        public int? RetryTimeout { get; set; }
        public int? DebugContentLimit { get; set; }

        
        public static implicit operator MethodOptions(string method) => new MethodOptions
        {
            MethodApi = method,
        };

        public MethodOptions With(params (string name, object value)[] values)
        {
            var replacedMethodApi = MethodApi;

            foreach (var (name, value) in values)
                replacedMethodApi = replacedMethodApi.Replace($"{{{name}}}", value.ToString());

            return new MethodOptions
            {
                MethodApi = replacedMethodApi,
                RequestTimeout = RequestTimeout,
                RetryCount = RetryCount,
                RetryTimeout = RetryTimeout,
                DebugContentLimit = DebugContentLimit,
            };
        }
    }

    public class MethodArgs
    {
        public string Host { get; set; }
        public  MethodOptions Method { get; set; }
        public  QueryArgsType QueryArgsType { get; set; }
        public Func<string> GetBeelineAtsToken { get; set; } = null;
        public Func<string> GetAuthorizationToken { get; set; } = null;
        public Func<string> GetJwtBearerToken { get; set; } = null;
        public Func<(string, string)> GetBasicAuth { get; set; } = null;
        public HttpClientCase? UseThrowCase { get; set; } = null;
        public bool IsOkOnly { get; set; } = false;
        public CancellationToken CancellationToken { get; set; } = default;
    }

    public enum QueryArgsType
    {
        QueryString,
        JsonBody
    }
}
