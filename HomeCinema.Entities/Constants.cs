using System;

namespace HomeCinema.Entities
{
    //TODO: Move to another assembly, should not be public
    public static class Users
    {
        public static User[] Defaults => new[]
        {
            SystemAdministratorUser

        };

        internal static readonly User SystemAdministratorUser = new User
        {
            ID = 1,
            Username = "admin",
            Email = "admin@undefined.com",
            HashedPassword = "vnBggdV0Ipy0m2OzuGpVYMYomYt13lO5Veeu+gH5aeQ=", // Initial Password: test
            Salt = "EmEJhNgyx1tjpVZkCLy2cQ==",
            IsLocked = false,
            IsSystemDefault = true,
            DateCreated = DateTime.Now
        };
    }

    public static class RoleClaims
    {
        public static RoleClaim[] Defaults => new[]
        {
            new RoleClaim
            {
                RoleId = Roles.SystemAdministratorRole.ID,
                ClaimId = Claims.UserAdminClaim.ID,
                IsSystemDefault = true
            },
            new RoleClaim
            {
                RoleId = Roles.SystemAdministratorRole.ID,
                ClaimId = 2,
                IsSystemDefault = true
            },
            new RoleClaim
            {
                RoleId = Roles.SystemAdministratorRole.ID,
                ClaimId = 3,
                IsSystemDefault = true
            },
            new RoleClaim
            {
                RoleId = Roles.SystemAdministratorRole.ID,
                ClaimId = 4,
                IsSystemDefault = true
            },
            new RoleClaim
            {
                RoleId = Roles.RentAdminRole.ID,
                ClaimId = 4,
                IsSystemDefault = true
            }
        };
    }

    public static class Claims
    {
        public static Claim[] All => new[]
        {
            UserAdminClaim,
            new Claim {ID = 2, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = MovieRead, Description = "Read movies and movie details"},
            new Claim {ID = 3, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = MovieCreate, Description = "Create new movies"},
            new Claim {ID = 4, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = MovieRent, Description = "Rent existing movies"},
        };

        public static readonly Claim UserAdminClaim = new Claim { ID = 1, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = UserAdmin, Description = "Read and update users" };

        public const string UserAdmin = "user.admin";
        public const string MovieRead = "movie.read";
        public const string MovieCreate = "movie.create";
        public const string MovieRent = "movie.rent";
    }

    public static class ClaimTypes
    {
        public const string Permission = "Permission";
    }

    public static class UserRoles
    {
        public static UserRole[] Defaults => new[]
        {
            new UserRole
            {
                UserId = Users.SystemAdministratorUser.ID,
                RoleId = Roles.SystemAdministratorRole.ID,
                IsSystemDefault = true
            }
        };
    }

    public static class Roles
    {
        public static Role[] Defaults => new[]
        {
            SystemAdministratorRole,
            RentAdminRole,
            GuestRole,
        };

        public const string SystemAdministrator = "System Admin";
        public const string RentAdmin = "Rent Admin";
        public const string Guest = "Guest";

        public static readonly Role SystemAdministratorRole = new Role { ID = 1, IsSystemDefault = true, Name = SystemAdministrator, Description = "System administration role" };
        public static readonly Role RentAdminRole = new Role { ID = 2, IsSystemDefault = false, Name = RentAdmin, Description = "Role for rent admins" };
        public static readonly Role GuestRole = new Role { ID = 3, IsSystemDefault = true, Name = Guest };
    }
}