using System;
using System.Collections.Generic;

namespace HomeCinema.Entities
{
    /// <summary>
    ///     HomeCinema User Account
    /// </summary>
    public class User : IEntityBase, ISystemDefault
    {
        public User()
        {
            this.UserRoles = new List<UserRole>();
        }

        public int ID { get; set; }

        public string Username { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public string Salt { get; set; }

        public bool IsLocked { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public bool IsSystemDefault { get; set; }
    }
}