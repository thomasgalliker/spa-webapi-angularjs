using SecurityClaim = System.Security.Claims.Claim;
using SecurityClaimTypes = System.Security.Claims.ClaimTypes;

namespace HomeCinema.Entities
{
    public static class ClaimExtensions
    {
        public static SecurityClaim ToSecurityClaim(this Claim claim)
        {
            return new SecurityClaim(claim.ClaimType, claim.ClaimValue);
        }

        public static SecurityClaim ToSecurityRoleClaim(this Role role)
        {
            return new SecurityClaim(SecurityClaimTypes.Role, role.Name);
        }

        public static SecurityClaim ToSecurityUserClaim(this User user)
        {
            return new SecurityClaim(type: SecurityClaimTypes.Name, value: user.Username);
        }

        public static Claim ToClaim(this SecurityClaim claim, int id)
        {
            return new Claim
            {
                ID = id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
        }
    }
}