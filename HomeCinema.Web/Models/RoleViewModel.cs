using System.Collections.Generic;

namespace HomeCinema.Web.Models
{
    public class RoleViewModel
    {
        public RoleViewModel()
        {
            this.Claims = new HashSet<ClaimViewModel>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<ClaimViewModel> Claims { get; set; }
    }
}