using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using HomeCinema.Data.Extensions;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services.Abstract;
using HomeCinema.Services.Utilities;

using Claim = HomeCinema.Entities.Claim;

namespace HomeCinema.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IEntityBaseRepository<User> userRepository;
        private readonly IEntityBaseRepository<Role> roleRepository;
        private readonly IEntityBaseRepository<UserRole> userRoleRepository;
        private readonly IEncryptionService encryptionService;
        private readonly IUnitOfWork unitOfWork;

        public MembershipService(
            IEntityBaseRepository<User> userRepository,
            IEntityBaseRepository<Role> roleRepository,
            IEntityBaseRepository<UserRole> userRoleRepository,
            IEncryptionService encryptionService,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.encryptionService = encryptionService;
            this.unitOfWork = unitOfWork;
        }

        #region IMembershipService Implementation

        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipContext = new MembershipContext();

            var user = this.userRepository.GetSingleByUsername(username);
            if (user != null && this.IsUserValid(user, password))
            {
                membershipContext.User = user;

                var userRoles = this.GetUserRoles(user.Username).ToArray();
                var roleClaims = userRoles.SelectMany(this.GetClaims).Distinct().ToArray();

                var claims = new List<System.Security.Claims.Claim>(1 + userRoles.Length + roleClaims.Length)
                {
                    user.ToSecurityUserClaim()
                };

                claims.AddRange(roleClaims.Select(c => c.ToSecurityClaim()));
                claims.AddRange(userRoles.Select(r => r.ToSecurityRoleClaim()));

                var claimsIdentity = new ClaimsIdentity(claims, authenticationType: "Custom");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                membershipContext.Principal = claimsPrincipal;
            }

            return membershipContext;
        }

        public User CreateUser(string username, string password, string email, params Role[] roles)
        {
            var existingUser = this.userRepository.GetSingleByUsername(username);
            if (existingUser != null)
            {
                throw new Exception("Username is already in use");
            }

            var passwordSalt = this.encryptionService.CreateSalt();

            var user = new User
            {
                Username = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = this.encryptionService.EncryptPassword(password, passwordSalt),
                DateCreated = DateTime.Now
            };

            this.userRepository.Add(user);

            //this.unitOfWork.Commit();

            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    this.AddUserToRole(user, role.ID);
                }
            }

            this.unitOfWork.Commit();

            return user;
        }

        public User GetUser(int userId)
        {
            return this.userRepository.GetSingle(userId);
        }

        public ICollection<User> GetUsers()
        {
            return this.userRepository.GetAll().ToList();
        }

        public List<Role> GetUserRoles(string username)
        {
            var result = new List<Role>();

            var existingUser = this.userRepository.GetSingleByUsername(username);
            if (existingUser != null)
            {
                foreach (var userRole in existingUser.UserRoles)
                {
                    result.Add(userRole.Role);
                }
            }

            return result.Distinct().ToList();
        }

        public List<Claim> GetClaims(Role role)
        {
            var result = new List<Claim>();

            var existingRole = this.roleRepository.GetAll().SingleOrDefault(r => r.ID == role.ID);
            if (existingRole != null)
            {
                foreach (var roleClaim in existingRole.RoleClaims)
                {
                    result.Add(roleClaim.Claim);
                }
            }

            return result.Distinct().ToList();
        }

        #endregion

        #region Helper methods

        private void AddUserToRole(User user, int roleId)
        {
            var role = this.roleRepository.GetSingle(roleId);
            if (role == null)
            {
                throw new Exception("Role doesn't exist.");
            }

            var userRole = new UserRole
            {
                RoleId = role.ID,
                UserId = user.ID
            };
            this.userRoleRepository.Add(userRole);
        }

        private bool IsPasswordValid(User user, string password)
        {
            return string.Equals(this.encryptionService.EncryptPassword(password, user.Salt), user.HashedPassword);
        }

        private bool IsUserValid(User user, string password)
        {
            if (this.IsPasswordValid(user, password))
            {
                return !user.IsLocked;
            }

            return false;
        }

        #endregion
    }
}