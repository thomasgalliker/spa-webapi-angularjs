namespace HomeCinema.Entities
{
    public static class ClaimTypes
    {
        public const string Permission = "Permission";
    }

    public static class Claims
    {
        public static Claim[] All => new[]
        {
            new Claim {ID = 1, ClaimType = ClaimTypes.Permission, ClaimValue = MovieRead, Description = "Read movies and movie details"},
            new Claim {ID = 2, ClaimType = ClaimTypes.Permission, ClaimValue = MovieCreate, Description = "Create new movies"},
            new Claim {ID = 3, ClaimType = ClaimTypes.Permission, ClaimValue = MovieRent, Description = "Rent existing movies"},
            new Claim {ID = 4, ClaimType = ClaimTypes.Permission, ClaimValue = UserAdmin, Description = "Read and update users"},
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
            new Role { ID = 1, Name = SystemAdmin, Description = "System administration role"},
            new Role { ID = 2, Name = RentAdmin, Description = "Role for rent admins" },
            GuestRole,
        };

        public const string SystemAdmin = "System Admin";
        public const string RentAdmin = "Rent Admin";
        public const string Guest = "Guest";
        public static readonly Role GuestRole = new Role { ID = 3, Name = Guest };
    }
}