using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;

namespace HomeCinema.Data.Tests
{
    public abstract class WebApiTestBase
    {
        protected WebApiTestBase(IContainer container)
        {
            this.HttpConfiguration = new HttpConfiguration
            {
                DependencyResolver = new AutofacWebApiDependencyResolver(container)
            };
        }

        protected HttpConfiguration HttpConfiguration { get; }
    }
}