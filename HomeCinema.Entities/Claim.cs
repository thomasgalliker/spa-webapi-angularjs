
namespace HomeCinema.Entities
{
    public class Claim : IEntityBase, ISystemDefault
    {
        public Claim()
        {
            this.ClaimType = ClaimTypes.Permission;
        }

        public int ID { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public string Description { get; set; }

        public bool IsSystemDefault { get; set; }
    }
}