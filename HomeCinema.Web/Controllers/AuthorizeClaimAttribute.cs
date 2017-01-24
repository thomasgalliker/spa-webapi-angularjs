using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;

using ClaimTypes = HomeCinema.Entities.ClaimTypes;

namespace HomeCinema.Web.Controllers
{
    public class AuthorizeClaimAttribute : AuthorizeAttribute //TODO : Use AuthorizationFilterAttribute
    {
        private readonly string claimType;
        private string claims;
        private string[] claimsSplit;

        public AuthorizeClaimAttribute(string claims) : this(claims, ClaimTypes.Permission)
        {
            this.Claims = claims;
        }

        public AuthorizeClaimAttribute(string claims, string claimType)
        {
            this.claimType = claimType;
            this.Claims = claims;
        }

        public AuthorizeClaimAttribute(params string[] claims)
        {
            this.claimsSplit = claims;
        }

        private string Claims
        {
            get
            {
                return this.claims ?? string.Empty;
            }
            set
            {
                this.claims = value;
                this.claimsSplit = SplitString(value);
            }
        }

        private static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original))
                return new string[0];
            return original.Split(',').Select(piece => new
            {
                piece = piece,
                trimmed = piece.Trim()
            }).Where(s => !string.IsNullOrEmpty(s.trimmed)).Select(param0 => param0.trimmed).ToArray();
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var principal = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;
            
            return principal != null && 
                   principal.Identity != null && 
                   principal.Identity.IsAuthenticated &&
                   this.AllClaimsContained(principal);
                   
        }

        private bool AllClaimsContained(ClaimsPrincipal claimsPrincipal)
        {
            if (this.claimsSplit.Length >= 0)
            {
                  return this.claimsSplit.Any(claimValue =>
                    claimsPrincipal.HasClaim(c => 
                        c.Type == this.claimType &&
                        c.Value == claimValue));
            }

            return false;
        }
    }
}