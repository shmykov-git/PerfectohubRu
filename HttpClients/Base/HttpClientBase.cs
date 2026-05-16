using Microsoft.Extensions.Options;
using Perfecto.Deploy.Extensions;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Shared.Exceptions;
using Shared.Exceptions.Cases;
using Shared.HttpClients.Options.Base;
using Shared.Libraries;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shared.HttpClients.Base
{
    public abstract class HttpClientBase
    {
        private readonly HttpClient client;
        protected readonly IServiceProvider sp;
        private readonly HttpClientOptions options;
        private string httpClientName;
        private string name;
        private Random rnd = new Random();

        protected HttpClientBase(
            HttpClient client, 
            IServiceProvider sp,
            IOptions<HttpClientOptions> options
            )
        {
            this.client = client;
            this.sp = sp;
            this.options = options.Value;
            httpClientName = GetType().Name;
            name = httpClientName.Replace("HttpClient", "");
        }

        protected Task<TView> PostAsync<TView>(object query, MethodArgs args) => Request<TView>(HttpMethod.Post, query, args);
        protected Task<TView> PostAsync<TView>(MethodArgs args) => Request<TView>(HttpMethod.Post, null, args);
        protected Task<TView> PutAsync<TView>(object query, MethodArgs args) => Request<TView>(HttpMethod.Put, query, args);
        protected Task<TView> GetAsync<TView>(object query, MethodArgs args) => Request<TView>(HttpMethod.Get, query, args);
        protected Task<TView> GetAsync<TView>(MethodArgs args) => Request<TView>(HttpMethod.Get, null, args);

        private HttpRequestMessage CreateHttpRequestMessage(HttpMethod httpMethod, string url, MethodArgs args, object query, uint requestId, out string body)
        {
            body = null;
            var requestMessage = new HttpRequestMessage(httpMethod, url);

            if (query == null)
            {
                //requestMessage.Content = new StringContent("", Encoding.UTF8, Values.MimeType.ApplicationJson);
            }
            else
                switch (args.QueryArgsType)
                {
                    case QueryArgsType.JsonBody:
                        var qBody = query.ToJsonStr();

                        requestMessage.Content = new StringContent(qBody, Encoding.UTF8, Values.MimeType.ApplicationJson);
                        body = qBody;
                        break;
                    case QueryArgsType.QueryString:
                        //requestMessage.Content = new StringContent("", Encoding.UTF8, Values.MimeType.ApplicationJson);
                        break;
                    default:
                        //requestMessage.Content = new StringContent("", Encoding.UTF8, Values.MimeType.ApplicationJson);
                        break;
                }

            UseHttpClientAuthorization(requestMessage, args, requestId);

            return requestMessage;
        }

        private async Task<TView> Request<TView>(HttpMethod httpMethod, object query, MethodArgs args)
        {
            if (args.Method?.MethodApi == null)
                throw new ArgumentNullException("HttpClient method cannot be null");

            var requestId = (uint)rnd.Next();
            //Debug.WriteLine($"HttpClient {name} {requestId} {httpMethod.Method} {args.QueryArgsType.ToJsonStr()} {query.ToJsonStr()} {args.Method.ToJsonStr()}");

            if (args.Host != null)
                client.BaseAddress = new Uri(args.Host);

            var methodApi = args.Method.MethodApi;

            var url = methodApi;

            if (args.QueryArgsType == QueryArgsType.QueryString)
                url = query == null
                    ? methodApi
                    : $"{methodApi}?{query.ToJsonQueryStr()}";

            // using policy with retry time, retry count, attempt logging
            var policy = GetPolicy(args.Method, requestId);
            HttpResponseMessage response;
            string body = null;

            try
            {
                response = await policy.ExecuteAsync(async ct =>
                {
                    // create request for each request try
                    using (var requestMessage = CreateHttpRequestMessage(httpMethod, url, args, query, requestId, out body))
                    {
                        return await client.SendAsync(requestMessage, ct);
                    }
                }, args.CancellationToken);
            }
            catch (HttpClientException e) when (e.Case == HttpClientCase.NoAuthToken)
            {
                if (args.UseThrowCase != null)
                    throw new HttpClientException(args.UseThrowCase.Value, e.Message);

                return default;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: HttpClient {name} {requestId} Error {httpMethod.Method} {url} {body.ToJsonStr()}");

                if (args.UseThrowCase != null)
                    throw new HttpClientException(args.UseThrowCase.Value, "Execution", ex);

                return default;
            }

            // if we stop the request by some reason
            if (args.CancellationToken.IsCancellationRequested)
            {
                Debug.WriteLine($"HttpClient {name} {requestId} CancellationRequested");

                if (args.UseThrowCase != null)
                    throw new HttpClientException(args.UseThrowCase.Value, "CancellationRequested");

                return default;
            }

            // cannot get correct response after many repeats
            if (!response.IsSuccessStatusCode)
            {            
                string errorContent;

                try
                {
                    errorContent = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    errorContent = $"[Cannot read error content. {ex.Message}]";
                }

                Debug.WriteLine($"HttpClient {name} {requestId} Error {response.StatusCode} {httpMethod.Method} {url} {body.ToJsonStr()} -> {errorContent.ToJsonStr()}");

                if (args.UseThrowCase != null)
                    throw new HttpClientException(args.UseThrowCase.Value, response.StatusCode, $"Error {response.StatusCode}", errorContent);

                return default;
            }

            if (args.IsOkOnly)
            {
                if (typeof(TView) != typeof(bool))
                    throw new ArgumentException($"HttpClient {name} {httpMethod.Method} {nameof(TView)} must be of type bool when using {nameof(args.IsOkOnly)}");

                return (TView)(object)true;
            }

            var contentStr = "[Cannot read content]";

            try
            {
                contentStr = await response.Content.ReadAsStringAsync();

                var view = typeof(TView) == typeof(string) 
                                ? (TView)(object)contentStr 
                                : contentStr.FromJsonStr<TView>();
            
                var debugContentLimit = args.Method.DebugContentLimit ?? options.DebugContentLimit;

                //if (debugContentLimit == null)
                //    Debug.WriteLine($"HttpClient {name} {requestId} {contentStr.Length} {contentStr.ToJsonStr()}");
                //else
                //    Debug.WriteLine($"HttpClient {name} {requestId} {contentStr.Length} {contentStr.Substring(0, Math.Min(contentStr.Length, debugContentLimit.Value)).ToJsonStr()}");

                return view;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: HttpClient {name} {requestId} format error '{contentStr.ToJsonStr()}'");

                if (args.UseThrowCase != null)
                    throw new HttpClientException(args.UseThrowCase.Value, $"Error {response.StatusCode}", ex);

                return default;
            }
        }

        public void UseHttpClientAuthorization(HttpRequestMessage requestMessage, MethodArgs args, uint requestId)
        {
            if (args.GetBeelineAtsToken != null)
            {
                var token = args.GetBeelineAtsToken();

                if (token == null)
                {
                    Debug.WriteLine($"WARN: HttpClient {name} {requestId} BeelineAtsToken is empty");

                    throw new HttpClientException(HttpClientCase.NoAuthToken, "BeelineAtsToken is empty");
                }

                requestMessage.Headers.Add(Values.HeaderName.BeelineAtsToken, token);
            }

            if (args.GetAuthorizationToken != null)
            {
                var token = args.GetAuthorizationToken();

                if (token == null)
                {
                    Debug.WriteLine($"WARN: HttpClient {name} {requestId} Authorization token is empty");

                    throw new HttpClientException(HttpClientCase.NoAuthToken, "Authorization token is empty");
                }

                requestMessage.Headers.Add(Values.HeaderName.Authorization, token);
            }

            if (args.GetJwtBearerToken != null)
            {
                var token = args.GetJwtBearerToken();

                if (token == null)
                {
                    Debug.WriteLine($"WARN: HttpClient {name} {requestId} Bearer is empty");

                    throw new HttpClientException(HttpClientCase.NoAuthToken, "Bearer token is empty");
                }

                requestMessage.Headers.Add(Values.HeaderName.Authorization, $"{Values.AuthSchema.Bearer} {token}");
            }

            if (args.GetBasicAuth != null)
            {
                var (username, password) = args.GetBasicAuth();

                if (username == null || password == null)
                {
                    Debug.WriteLine($"WARN: HttpClient {name} {requestId} basic auth credintials is empty");

                    throw new HttpClientException(HttpClientCase.NoAuthToken, "Basic auth credintials is empty");
                }

                string credentials = $"{username}:{password}";
                string encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
                requestMessage.Headers.Add(Values.HeaderName.Authorization, $"Basic {encodedCredentials}");
            }
        }

        private IAsyncPolicy<HttpResponseMessage> GetPolicy(MethodOptions method, uint requestId)
        {
            TimeSpan GetSleepDuration(int retryNumber) => TimeSpan.FromSeconds(method.RetryTimeout ?? options.RetryTimeout ?? 1);
            var retryCount = method.RetryCount ?? options.RetryCount ?? 0;

            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutException>()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(retryCount, GetSleepDuration, (outcome, timespan, attempt, context) =>
                {
                    var methodApi = method.MethodApi;
                    Debug.WriteLine($"WARN: HttpClient {name} {requestId} #{attempt}, repeat after {timespan.TotalSeconds} sec, method '{methodApi}'");
                });

            if (method.RequestTimeout == null)
                return retryPolicy;

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(method.RequestTimeout.Value, TimeoutStrategy.Optimistic);
            var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

            return combinedPolicy;
        }
    }
}
