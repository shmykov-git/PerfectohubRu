using Shared.Exceptions.Cases;
using System;
using System.Collections.Generic;
using System.Net;

namespace Shared.Exceptions
{
    public class HttpClientException : Exception
    {
        public HttpClientCase Case { get; } = HttpClientCase.None;
        public HttpStatusCode? Status { get; } = null;
        public string ErrorContent { get; } = null;

        public Dictionary<string, object> Args { get; } = new Dictionary<string, object>();

        public HttpClientException(HttpClientCase @case, string message, string errorContent)
            : this(@case, null, message, null, null, errorContent)
        {
        }

        public HttpClientException(HttpClientCase @case, HttpStatusCode status, string message, string errorContent)
            : this(@case, status, message, null, null, errorContent)
        {
        }

        public HttpClientException(HttpClientCase @case, string message, Exception innerException = null, string errorContent = null)
            : this(@case, null, message, null, innerException, null)
        {
        }

        public HttpClientException(HttpClientCase @case, HttpStatusCode status, string message, Exception innerException = null, string errorContent = null)
            : this(@case, status, message, null, innerException, errorContent)
        {
        }

        public HttpClientException(
            HttpClientCase @case,
            HttpStatusCode? status = null,
            string message = null,
            Dictionary<string, object> args = null,
            Exception innerException = null,
            string errorContent = null
            ) : base(message, innerException)
        {
            Case = @case;
            Status = status;
            Args = args ?? new Dictionary<string, object>();
            ErrorContent = errorContent;
        }
    }
}
