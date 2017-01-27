using Autofac;
using Autofac.Core;

namespace HomeCinema.Data.Tests
{
    public static class TestHelper
    {
        public static IContainer BuildContainer(IModule module)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);
            return builder.Build();
        }
    }
}
