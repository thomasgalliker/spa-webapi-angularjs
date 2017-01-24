﻿using System.Collections.Generic;

namespace HomeCinema.Entities
{
    /// <summary>
    ///     HomeCinema Role
    /// </summary>
    public class Role : IEntityBase
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    }
}