namespace HomeCinema.Entities
{
    public class RoleClaim : IEntityBase
    {
        public int ID { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public int ClaimId { get; set; }

        public virtual Claim Claim { get; set; }

        //public DateTime? ValidFrom { get; set; }

        //public DateTime? ValidTo { get; set; }
    }
}