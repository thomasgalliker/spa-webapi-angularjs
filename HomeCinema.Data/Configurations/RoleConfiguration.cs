using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
    public class RoleConfiguration : EntityBaseConfiguration<Role>
    {
        public RoleConfiguration()
        {
            this.Property(ur => ur.Name).IsRequired().HasMaxLength(50);
            this.Property(ur => ur.Description).IsOptional().HasMaxLength(200);
            //this.HasMany(r => r.RolePermissions).WithRequired().HasForeignKey(p => p.Id);
        }
    }
}