using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
    public class ClaimConfiguration : EntityBaseConfiguration<Claim>
    {
        public ClaimConfiguration()
        {
            this.Property(ur => ur.ClaimType).IsRequired().HasMaxLength(50);
            this.Property(ur => ur.ClaimValue).IsRequired().HasMaxLength(50);
            this.Property(ur => ur.Description).IsOptional().HasMaxLength(200);
        }
    }
}