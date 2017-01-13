using System.Web.Http;
using HomeCinema.Web.Mappings;

namespace HomeCinema.Web.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            // Configure Autofac
            AutofacWebapiConfig.Initialize(GlobalConfiguration.Configuration);

            //Configure AutoMapper
            AutoMapperConfiguration.Configure();
        }
    }
}