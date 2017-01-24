using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
    public class RoleClaimConfiguration : EntityBaseConfiguration<RoleClaim>
    {
        public RoleClaimConfiguration()
        {
            this.Property(ur => ur.RoleId).IsRequired();
            this.Property(ur => ur.ClaimId).IsRequired();

            //this.Ignore(d => d.Claim);
            //this.Ignore(d => d.Role);
        }
    }
}