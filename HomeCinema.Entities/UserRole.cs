namespace HomeCinema.Entities
{
    public class UserRole : IEntityBase, ISystemDefault
    {
        public int ID { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public virtual Role Role { get; set; }

        public bool IsSystemDefault { get; set; }
    }
}