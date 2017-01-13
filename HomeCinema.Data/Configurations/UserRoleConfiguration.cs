using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
    public class UserRoleConfiguration : EntityBaseConfiguration<UserRole>
    {
        public UserRoleConfiguration()
        {
            this.Property(ur => ur.UserId).IsRequired();
            this.Property(ur => ur.RoleId).IsRequired();
        }
    }
}