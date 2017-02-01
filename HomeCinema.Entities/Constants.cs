using System;

namespace HomeCinema.Entities
{
    public static class ClaimTypes
    {
        public const string Permission = "Permission";
    }

    //TODO: Move to another assembly, should not be public
    public static class Users
    {
        public static User[] Defaults => new[]
        {
            new User
            {
                Username = "admin",
                Email = "admin@undefined.com",
                HashedPassword = "vnBggdV0Ipy0m2OzuGpVYMYomYt13lO5Veeu+gH5aeQ=", // Initial Password: test
                Salt = "EmEJhNgyx1tjpVZkCLy2cQ==",
                IsLocked = false,
                IsSystemDefault = true,
                DateCreated = DateTime.Now
            }
        };
    }

    public static class Claims
    {
        public static Claim[] All => new[]
        {
            new Claim {ID = 1, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = MovieRead, Description = "Read movies and movie details"},
            new Claim {ID = 2, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = MovieCreate, Description = "Create new movies"},
            new Claim {ID = 3, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = MovieRent, Description = "Rent existing movies"},
            new Claim {ID = 4, ClaimType = ClaimTypes.Permission, IsSystemDefault = true, ClaimValue = UserAdmin, Description = "Read and update users"},
        };

        public const string MovieRead = "movie.read";
        public const string MovieCreate = "movie.create";
        public const string MovieRent = "movie.rent";
        public const string UserAdmin = "user.admin";
    }


    public static class Roles
    {
        public static Role[] Defaults => new[]
        {
            new Role { ID = 1, IsSystemDefault = true, Name = SystemAdmin, Description = "System administration role"},
            new Role { ID = 2, IsSystemDefault = true, Name = RentAdmin, Description = "Role for rent admins" },
            GuestRole,
        };

        public const string SystemAdmin = "System Admin";
        public const string RentAdmin = "Rent Admin";
        public const string Guest = "Guest";
        public static readonly Role GuestRole = new Role { ID = 3, Name = Guest };
    }
}