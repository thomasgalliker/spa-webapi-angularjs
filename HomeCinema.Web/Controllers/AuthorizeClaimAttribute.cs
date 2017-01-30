using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using HomeCinema.Web.Infrastructure.ExceptionHandling;
using HomeCinema.Web.Infrastructure.Extensions;

using ClaimTypes = HomeCinema.Entities.ClaimTypes;

namespace HomeCinema.Web.Controllers
{
    public class AuthorizeClaimAttribute : AuthorizationFilterAttribute
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

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            if (!actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
            }

            return true;
        }

        private static bool IsAuthenticated(IPrincipal principal)
        {
            return principal != null && principal.Identity != null && principal.Identity.IsAuthenticated;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (SkipAuthorization(actionContext))
            {
                return;
            }

            var principal = actionContext.ControllerContext.RequestContext.Principal as ClaimsPrincipal;
            if (IsAuthenticated(principal))
            {
                if (!this.AllClaimsContained(principal))
                {
                    this.HandleUnauthorizedRequest(actionContext);
                }
            }
            else
            {
                this.HandleUnauthenticatedRequest(actionContext);
            }
        }

        protected virtual void HandleUnauthenticatedRequest(HttpActionContext actionContext)
        {
            var request = actionContext.ControllerContext.Request;
            var errorMessage = new ExceptionDto
            {
                CorrelationId = request.GetCorrelationId(),
                RequestMethod = request.Method.AsString(),
                RequestUri = request.RequestUri.AsString(),
                Message = $"Identity with name '{actionContext.RequestContext.Principal.Identity.Name}' is not authenticated."
            };

            // 401 Unauthorized means 'not authenticated', see here: http://stackoverflow.com/questions/3297048/403-forbidden-vs-401-unauthorized-http-responses
            var response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.Unauthorized, errorMessage);
            actionContext.Response = response;
        }

        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            var request = actionContext.ControllerContext.Request;
            var errorMessage = new ExceptionDto
            {
                CorrelationId = request.GetCorrelationId(),
                RequestMethod = request.Method.AsString(),
                RequestUri = request.RequestUri.AsString(),
                Message = $"Identity with name '{actionContext.RequestContext.Principal.Identity.Name}' is not authorized to access resource {request.RequestUri.AsString()}."
            };

            var response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.Forbidden, errorMessage);
            actionContext.Response = response;
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