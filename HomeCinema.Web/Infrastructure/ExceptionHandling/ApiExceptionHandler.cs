using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

using HomeCinema.Web.Infrastructure.Extensions;

namespace HomeCinema.Web.Infrastructure.ExceptionHandling
{
    internal class ApiExceptionHandler : ExceptionHandler
    {
        public override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            var exception = new ExceptionDto
            {
                CorrelationId = context.Request.GetCorrelationId(),
                RequestMethod = context.Request.Method.AsString(),
                RequestUri = context.Request.RequestUri.AsString(),
                Message = context.Exception.Message,
                Exception = context.Exception.ToString()
            };

            var response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, exception);
            context.Result = new ExceptionHttpActionResult(response);

            return base.HandleAsync(context, cancellationToken);
        }

        private class ExceptionHttpActionResult : IHttpActionResult
        {
            private readonly HttpResponseMessage response;

            public ExceptionHttpActionResult(HttpResponseMessage response)
            {
                this.response = response;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(this.response);
            }
        }
    }

    public class ExceptionDto
    {
        public string RequestMethod { get; set; }

        public string RequestUri { get; set; }

        public Guid CorrelationId { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }
    }
}