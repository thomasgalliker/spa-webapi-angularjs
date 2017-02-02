using System.Collections.Generic;

using HomeCinema.Entities;
using HomeCinema.Services.Utilities;

namespace HomeCinema.Services.Abstract
{
    public interface IMembershipService
    {
        MembershipContext ValidateUser(string username, string password);

        User CreateUser(string username, string password, string email, params Role[] roles);

        User GetUser(int userId);

        ICollection<User> GetUsers();

        List<Role> GetUserRoles(string username);

        ICollection<Role> GetRoles();

        void UpdateUser(User user, User updateUser);

        bool IsExistingUser(int userId);

        void DeleteUser(User user);

        ICollection<Claim> GetClaims();

        ICollection<Role> GetRolesByClaimId(int claimId);

        Role GetRole(int roleId);

        void UpdateRole(Role role, Role updateRole);
    }
}