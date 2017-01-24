using System.Web.Http;
using System.Web.Http.ExceptionHandling;

using HomeCinema.Web.Filters;
using HomeCinema.Web.Infrastructure.ExceptionHandling;
using HomeCinema.Web.Infrastructure.Logging;
using HomeCinema.Web.Infrastructure.MessageHandlers;

using Tracing;

namespace HomeCinema.Web
{
    public static class WebApiConfig
    {
        static WebApiConfig()
        {
            Tracer.SetFactory(new DebugTracerFactory());
        }

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new AuthenticationHandler());
            config.MessageHandlers.Add(new MessageLoggingHandler());

            config.Filters.Add(new ValidateModelAttribute());
            //config.Filters.Add(new ClaimsFilter());

            config.Services.Replace(typeof(IExceptionHandler), new ApiExceptionHandler());
            config.Services.Add(typeof(IExceptionLogger), new ApiExceptionLogger());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}", 
                defaults: new { id = RouteParameter.Optional });
        }
    }
}