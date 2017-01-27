using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using FluentAssertions;

using HomeCinema.Entities;
using HomeCinema.Web.Mappings;
using HomeCinema.Web.Models;

using Xunit;

namespace HomeCinema.Web.Tests
{
    public class MappingTests
    {
        public MappingTests()
        {
            AutoMapperConfiguration.Configure();
        }

        [Fact]
        public void ShouldMapUserToUserViewModel()
        {
            // Arrange
            var users = new List<User> { new User { Username = "Thomas", Email = "mail@test.com" } };

            // Act
            var userViewModels = Mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);

            // Assert

        }

        [Fact]
        public void ShouldMapUserToUserDetailViewModel()
        {
            // Arrange
            var claim = Claims.All[0];
            claim.ID = 1;

            var role = Roles.GuestRole;
            role.ID = 1;
            role.RoleClaims = new List<RoleClaim> { new RoleClaim { Claim = claim } };

            var users = new List<User>
            {
                new User
                {
                    ID = 1,
                    Username = "Thomas",
                    Email = "mail@test.com",
                    UserRoles = new List<UserRole> { new UserRole { Role = Roles.GuestRole } }
                }
            };

            // Act
            var userDetailViewModels = Mapper.Map<IEnumerable<User>, IEnumerable<UserDetailViewModel>>(users);

            // Assert
            var userDetailViewModel = userDetailViewModels.ElementAt(0);
            userDetailViewModel.Id.Should().Be(1);

            userDetailViewModel.Roles.Should().HaveCount(1);
            var roleViewModel = userDetailViewModel.Roles.ElementAt(0);
            roleViewModel.Id.Should().Be(1);

            userDetailViewModel.Claims.Should().HaveCount(1);
            var claimViewModel = userDetailViewModel.Claims.ElementAt(0);
            claimViewModel.Id.Should().Be(1);
        }

        [Fact]
        public void ShouldMapClaimToClaimViewModel()
        {
            // Arrange
            var claims = new List<Claim> { new Claim { ID = 1, ClaimType = "Mail", ClaimValue = "mail@test.com" } };

            // Act
            var claimViewModels = Mapper.Map<IEnumerable<Claim>, IEnumerable<ClaimViewModel>>(claims);

            // Assert
            claimViewModels.Should().NotBeNullOrEmpty();
            var claimViewModel = claimViewModels.ElementAt(0);
            claimViewModel.Id.Should().Be(1);
            claimViewModel.ClaimType.Should().Be("Mail");
            claimViewModel.ClaimValue.Should().Be("mail@test.com");
        }
    }
}
