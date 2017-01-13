using System;
using System.Collections.Generic;

namespace HomeCinema.Entities
{
    /// <summary>
    ///     HomeCinema User Account
    /// </summary>
    public class User : IEntityBase
    {
        public User()
        {
            this.UserRoles = new List<UserRole>();
        }

        public int ID { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public string Salt { get; set; }

        public bool IsLocked { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}