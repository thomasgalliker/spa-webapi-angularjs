using System.Collections.Generic;

namespace HomeCinema.Web.Models
{
    public class ClaimViewModel
    {
        public ClaimViewModel()
        {
            this.Roles = new HashSet<RoleViewModel>();
        }

        public int Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public string Description { get; set; }

        public ICollection<RoleViewModel> Roles { get; set; }
    }
}