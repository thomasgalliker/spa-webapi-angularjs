using System;
using System.Net.Http;
using System.Text;

namespace HomeCinema.Web.Infrastructure.Extensions
{
    internal static class HttpRequestMessageExtensions
    {
        private const string NullReplacement = "<null>";

        internal static string AsString(this HttpMethod httpMethod)
        {
            return httpMethod?.ToString() ?? NullReplacement;
        }

        internal static string AsString(this Uri uri)
        {
            return uri?.OriginalString ?? NullReplacement;
        }

        internal static string AsString(this HttpRequestMessage request)
        {
            var message = new StringBuilder();
            message.Append("CorrelationId: ");
            message.AppendLine(request.GetCorrelationId().ToString());

            message.Append("Method: ");
            message.AppendLine(request.Method.AsString());

            message.Append("RequestUri: ");
            message.AppendLine(request.RequestUri.AsString());

            return message.ToString();
        }
    }
}