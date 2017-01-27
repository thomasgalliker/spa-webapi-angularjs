using Autofac;

using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;

using HomeCinema.Data.Repositories;

namespace HomeCinema.Data.Modularity
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterModule(new TracerModule());

            builder.RegisterGeneric(typeof(GenericRepository<>))
                   .As(typeof(IGenericRepository<>))
                   .AsSelf()
                   .InstancePerRequest();

            builder.RegisterType(typeof(UserRepository))
                  .As(typeof(IUserRepository))
                  .InstancePerRequest();

            builder.RegisterType(typeof(HomeCinemaContext))
                  .As(typeof(IHomeCinemaContext))
                  .As(typeof(IDbContext))
                  .InstancePerRequest();
        }
    }
}
