using System.Text;
using System.Threading.Tasks;

using Tracing;

namespace HomeCinema.Web.Infrastructure.Logging
{
    /// <summary>
    ///     Source: https://weblogs.asp.net/fredriknormen/log-message-request-and-response-in-asp-net-webapi
    /// </summary>
    public class MessageLoggingHandler : MessageHandler
    {
        private static readonly ITracer Tracer = Tracing.Tracer.Create(typeof(MessageLoggingHandler));
        private readonly Category logLevel;

        public MessageLoggingHandler(Category logLevel = Category.Debug)
        {
            this.logLevel = logLevel;
        }

        protected override async Task IncommingMessageAsync(string correlationId, string requestInfo, byte[] message)
        {
            await Task.Run(() => Tracer.Write(this.logLevel, BuildTraceMessage("Request", correlationId, requestInfo, message)));
        }

        protected override async Task OutgoingMessageAsync(string correlationId, string requestInfo, byte[] message)
        {
            await Task.Run(() => Tracer.Write(this.logLevel, BuildTraceMessage("Response", correlationId, requestInfo, message)));
        }

        private static string BuildTraceMessage(string prefix, string correlationId, string requestInfo, byte[] message)
        {
            var messageStringBuilder = new StringBuilder();
            messageStringBuilder.AppendLine($"{prefix}: {requestInfo}");
            messageStringBuilder.AppendLine($"CorrelationId: {correlationId}");
            messageStringBuilder.AppendLine($"Message ({message.Length} bytes): ");
            messageStringBuilder.AppendLine(Encoding.UTF8.GetString(message));

            return messageStringBuilder.ToString();
        }
    }
}