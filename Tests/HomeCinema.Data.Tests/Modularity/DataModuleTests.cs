using EntityFramework.Toolkit;
using EntityFramework.Toolkit.Core;

using FluentAssertions;
using HomeCinema.Data.Modularity;
using HomeCinema.Data.Repositories;
using HomeCinema.Data.Tests.Extensions;
using HomeCinema.Entities;

using Xunit;

namespace HomeCinema.Data.Tests.Modularity
{
    public class DataModuleTests : WebApiTestBase
    {
        public DataModuleTests()
            : base(TestHelper.BuildContainer(new DataModule()))
        {
        }

        [Fact]
        public void ShouldResolveAllDependencies()
        {
            // Arrange
            using (var dependencyScope = this.HttpConfiguration.DependencyResolver.BeginScope())
            {
                // Act
                var homeCinemaContext = dependencyScope.GetService<IHomeCinemaContext>();
                var userRepository = dependencyScope.GetService<IUserRepository>();
                var roleRepository = dependencyScope.GetService<IGenericRepository<Role>>();
                var userRoleRepository = dependencyScope.GetService<IGenericRepository<UserRole>>();

                // Assert
                homeCinemaContext.Should().BeOfType<HomeCinemaContext>();

                userRepository.Should().BeOfType<UserRepository>();
                userRepository.Context.Should().BeSameAs(homeCinemaContext);

                roleRepository.Should().BeOfType<GenericRepository<Role>>();
                roleRepository.Context.Should().BeSameAs(homeCinemaContext);

                userRoleRepository.Should().BeOfType<GenericRepository<UserRole>>();
                userRoleRepository.Context.Should().BeSameAs(homeCinemaContext);
            }
        }
    }
}
