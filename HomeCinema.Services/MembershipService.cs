using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using EntityFramework.Toolkit.Core;
using EntityFramework.Toolkit.Core.Extensions;

using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Services.Abstract;
using HomeCinema.Services.Utilities;

using Claim = HomeCinema.Entities.Claim;
using IUnitOfWork = HomeCinema.Data.Infrastructure.IUnitOfWork;

namespace HomeCinema.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly IUserRepository userRepository;
        private readonly IGenericRepository<Role> roleRepository;
        private readonly IGenericRepository<UserRole> userRoleRepository;
        private readonly IGenericRepository<RoleClaim> roleClaimRepository;
        private readonly IGenericRepository<Claim> claimRepository;
        private readonly IEncryptionService encryptionService;
        private readonly IUnitOfWork unitOfWork;

        public MembershipService(
            IUserRepository userRepository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IGenericRepository<RoleClaim> roleClaimRepository,
            IGenericRepository<Claim> claimRepository,
            IEncryptionService encryptionService,
            IUnitOfWork unitOfWork)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.roleClaimRepository = roleClaimRepository;
            this.claimRepository = claimRepository;
            this.encryptionService = encryptionService;
            this.unitOfWork = unitOfWork;
        }

        #region IMembershipService Implementation

        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipContext = new MembershipContext();

            var user = this.userRepository.Get().SingleOrDefault(u => u.Username == username);
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
            var existingUser = this.userRepository.GetByUsername(username);
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
            this.userRepository.Save();

            //this.unitOfWork.Commit(); // TODO: THis call is very problematic; can lead to situation where we have added a user, but without any role (in case 2nd commit fails)

            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    this.AddUserToRole(user, role.ID);
                }
            }

            this.roleRepository.Save();
            //this.unitOfWork.Commit();

            return user;
        }

        public User GetUser(int userId)
        {
            return this.userRepository.FindById(userId);
        }

        public ICollection<User> GetUsers()
        {
            return this.userRepository.GetAll().ToList();
        }

        public List<Role> GetUserRoles(string username)
        {
            var result = new List<Role>();

            var existingUser = this.userRepository.GetByUsername(username);
            if (existingUser != null)
            {
                foreach (var userRole in existingUser.UserRoles)
                {
                    result.Add(userRole.Role);
                }
            }

            return result.Distinct().ToList();
        }

        public ICollection<Role> GetRoles()
        {
            return this.roleRepository.GetAll().ToList();
        }

        public void UpdateUser(User user, User updateUser)
        {
            // Update scalar properties
            user.Email = updateUser.Email;
            user.Firstname = updateUser.Firstname;
            user.Lastname = updateUser.Lastname;
            user.IsLocked = updateUser.IsLocked;

            // Upate roles and claims
            var newRoleIds = updateUser.UserRoles.Select(ur => ur.RoleId).ToList();
            foreach (var userRole in user.UserRoles.ToList())
            {
                if (!newRoleIds.Contains(userRole.RoleId))
                {
                    // Cannot remove system default user-role assignment
                    if (user.IsSystemDefault && userRole.IsSystemDefault && userRole.Role.IsSystemDefault)
                    {
                        continue;
                    }

                    this.userRoleRepository.Remove(userRole);
                    user.UserRoles.Remove(userRole);
                }
            }

            foreach (var roleId in newRoleIds)
            {
                // Add the roles which are not in the list of user's roles
                if (user.UserRoles.All(ur => ur.RoleId != roleId))
                {
                    var newUserRole = new UserRole { UserId = user.ID, RoleId = roleId };
                    user.UserRoles.Add(newUserRole);
                }
            }

            // Save to database
            this.userRepository.Update(user);
            this.userRepository.Save();
        }

        public bool IsExistingUser(int userId)
        {
            return this.userRepository.Any(userId);
        }

        public void DeleteUser(User user)
        {
            this.userRepository.Remove(user);
            this.userRepository.Save();
        }

        public ICollection<Claim> GetClaims()
        {
            return this.claimRepository.GetAll().ToList();
        }

        public ICollection<Role> GetRolesByClaimId(int claimId)
        {
            return this.roleRepository.Get()
                .Where(r => r.RoleClaims.Any(rc => rc.ClaimId == claimId))
                .ToList();
        }

        public Role GetRole(int roleId)
        {
            return this.roleRepository.FindById(roleId);
        }

        public void UpdateRole(Role role, Role updateRole)
        {
            // Update scalar properties
            if (role.IsSystemDefault == false)
            {
                role.Name = updateRole.Name;
            }
            role.Description = updateRole.Description;

            // Upate claims
            var newClaimIds = updateRole.RoleClaims.Select(rc => rc.ClaimId).ToList();
            foreach (var roleClaim in role.RoleClaims.ToList())
            {
                if (!newClaimIds.Contains(roleClaim.ClaimId))
                {
                    // Cannot remove system default role-claim assignment
                    if (role.IsSystemDefault && roleClaim.IsSystemDefault && roleClaim.Claim.IsSystemDefault)
                    {
                        continue;
                    }

                    this.roleClaimRepository.Remove(roleClaim);
                    role.RoleClaims.Remove(roleClaim);
                }
            }

            foreach (var claimId in newClaimIds)
            {
                // Add the claims which are not in the list of role's claims
                if (role.RoleClaims.All(ur => ur.ClaimId != claimId))
                {
                    var newRoleClaim = new RoleClaim { RoleId = role.ID, ClaimId = claimId };
                    role.RoleClaims.Add(newRoleClaim);
                }
            }

            // Save to database
            this.roleRepository.Update(role);
            this.roleRepository.Save();
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
            var role = this.roleRepository.FindById(roleId);
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