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
            new Claim {ID = 1, ClaimType = ClaimTypes.Permission, ClaimValue = MovieRead},
            new Claim {ID = 2, ClaimType = ClaimTypes.Permission, ClaimValue = MovieCreate},
            new Claim {ID = 3, ClaimType = ClaimTypes.Permission, ClaimValue = MovieRent},
            new Claim {ID = 4, ClaimType = ClaimTypes.Permission, ClaimValue = UserAdmin},
        };

        public const string MovieRead = "user.admin";
        public const string MovieCreate = "user.admin";
        public const string MovieRent = "user.admin";
        public const string UserAdmin = "user.admin";
    }


    public static class Roles
    {
        public static Role[] Defaults => new[]
        {
            new Role { ID = 1, Name = Roles.SystemAdmin },
            new Role { ID = 2, Name = Roles.RentAdmin },
            GuestRole,
        };

        public const string SystemAdmin = "System Admin";
        public const string RentAdmin = "Rent Admin";
        public const string Guest = "Guest";
        public static readonly Role GuestRole = new Role { ID = 3, Name = Guest };
    }
}