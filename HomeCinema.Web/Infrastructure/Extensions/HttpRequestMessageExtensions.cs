using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Dependencies;

using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services.Abstract;

namespace HomeCinema.Web.Infrastructure.Extensions
{
    internal static class HttpRequestMessageExtensions
    {
        private const string NullReplacement = "<null>";

        internal static IMembershipService GetMembershipService(this HttpRequestMessage request)
        {
            return request.GetService<IMembershipService>();
        }

        internal static IEntityBaseRepository<T> GetDataRepository<T>(this HttpRequestMessage request) where T : class, IEntityBase, new()
        {
            return request.GetService<IEntityBaseRepository<T>>();
        }

        private static TService GetService<TService>(this HttpRequestMessage request)
        {
            IDependencyScope dependencyScope = request.GetDependencyScope();
            return dependencyScope.GetService<TService>();
        }

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