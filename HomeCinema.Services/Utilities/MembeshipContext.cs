using System.Security.Principal;

using HomeCinema.Entities;

namespace HomeCinema.Services.Utilities
{
    public class MembershipContext
    {
        public IPrincipal Principal { get; set; }

        public User User { get; set; }

        public bool IsValid()
        {
            return this.Principal != null;
        }
    }
}