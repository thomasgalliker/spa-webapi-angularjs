using System.Data.Entity;
using System.Reflection;
using System.Web.Http;

using Autofac;
using Autofac.Integration.WebApi;

using HomeCinema.Data;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Modularity;
using HomeCinema.Data.Repositories;
using HomeCinema.Services;
using HomeCinema.Services.Abstract;
using HomeCinema.Web.Infrastructure.Core;

using Tracing;
using Tracing.Integration.Autofac;

using IUnitOfWork = HomeCinema.Data.Infrastructure.IUnitOfWork;
using UnitOfWork = HomeCinema.Data.Infrastructure.UnitOfWork;

namespace HomeCinema.Web
{
    public class AutofacWebapiConfig
    {
        private static IContainer container;

        public static void Initialize(HttpConfiguration config)
        {
            Tracer.SetFactory(new DebugTracerFactory());
            container = RegisterServices(new ContainerBuilder());
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModule(new TracerModule());
            builder.RegisterModule(new DataModule());

            // EF HomeCinemaContext
            builder.RegisterType<HomeCinemaContext>() // TODO Move to Data layer
                   .As<DbContext>()
                   .InstancePerRequest();

            builder.RegisterType<DbFactory>() // TODO Move to Data layer
                .As<IDbFactory>()
                .InstancePerRequest();

            builder.RegisterType<UnitOfWork>() // TODO Move to Data layer
                .As<IUnitOfWork>()
                .InstancePerRequest();

            builder.RegisterGeneric(typeof(EntityBaseRepository<>))
                   .As(typeof(IEntityBaseRepository<>))
                   .InstancePerRequest();

            // Services
            builder.RegisterType<EncryptionService>()
                .As<IEncryptionService>()
                .InstancePerRequest();

            builder.RegisterType<MembershipService>()
                .As<IMembershipService>()
                .InstancePerRequest();

            // Generic Data Repository Factory
            builder.RegisterType<DataRepositoryFactory>()
                .As<IDataRepositoryFactory>()
                .InstancePerRequest();

            container = builder.Build();

            return container;
        }
    }
}