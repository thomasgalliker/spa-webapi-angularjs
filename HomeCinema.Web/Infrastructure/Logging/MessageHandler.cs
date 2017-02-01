using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeCinema.Web.Infrastructure.Logging
{
    public abstract class MessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = request.GetCorrelationId().ToString();
            var requestInfo = $"{request.Method} {request.RequestUri}";

            var requestMessage = await request.Content.ReadAsByteArrayAsync();

            await this.IncommingMessageAsync(correlationId, requestInfo, requestMessage);

            var response = await base.SendAsync(request, cancellationToken);

            byte[] responseMessage;

            if (response.IsSuccessStatusCode)
            {
                responseMessage = response.Content == null ? new byte[] { } : await response.Content.ReadAsByteArrayAsync();
            }
            else
            {
                responseMessage = Encoding.UTF8.GetBytes(response.ReasonPhrase);
            }

            await this.OutgoingMessageAsync(correlationId, requestInfo, responseMessage);

            return response;
        }

        protected abstract Task IncommingMessageAsync(string correlationId, string requestInfo, byte[] message);

        protected abstract Task OutgoingMessageAsync(string correlationId, string requestInfo, byte[] message);
    }
}