using System.Web.Http.ExceptionHandling;

using HomeCinema.Web.Infrastructure.Extensions;

using Tracing;

namespace HomeCinema.Web.Infrastructure.Logging
{
    /// <summary>
    ///     Global exception logger for api requests.
    ///     Source: http://stackoverflow.com/questions/25865610/global-exception-handling-in-web-api-2-1-and-nlog
    /// </summary>
    internal class ApiExceptionLogger : ExceptionLogger
    {
        private static readonly ITracer Tracer = Tracing.Tracer.Create(typeof(ApiExceptionLogger));

        public override void Log(ExceptionLoggerContext context)
        {
            var requestAsString = context.Request.AsString();
            var ex = context.Exception;
            var controllerContext = context.ExceptionContext.ControllerContext;
            if (controllerContext != null)
            {
                var controllerTracer = Tracing.Tracer.Create(controllerContext.Controller.GetType());
                controllerTracer.Exception(ex, $"An unhandled api exception occurred in {controllerContext.Controller.GetType().Name}.\n\r{requestAsString}");
            }
            else
            {
                Tracer.Exception(ex, $"An unhandled api exception occurred.\r\n{ex}");
            }
        }
    }
}