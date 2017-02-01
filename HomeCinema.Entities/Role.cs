using System.Collections.Generic;

namespace HomeCinema.Entities
{
    /// <summary>
    ///     HomeCinema Role
    /// </summary>
    public class Role : IEntityBase, ISystemDefault
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<RoleClaim> RoleClaims { get; set; }

        public bool IsSystemDefault { get; set; }
    }
}