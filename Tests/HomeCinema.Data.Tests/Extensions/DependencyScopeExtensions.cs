using System.Web.Http.Dependencies;

namespace HomeCinema.Data.Tests.Extensions
{
    internal static class DependencyScopeExtensions
    {
        internal static TService GetService<TService>(this IDependencyScope dependencyScope)
        {
            TService service = (TService)dependencyScope.GetService(typeof(TService));

            return service;
        }
    }
}