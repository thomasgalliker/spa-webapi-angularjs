using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using HomeCinema.Web.Infrastructure.ExceptionHandling;
using HomeCinema.Web.Infrastructure.Extensions;

namespace HomeCinema.Web.Controllers
{
    public class AuthorizeRoleAttribute : AuthorizationFilterAttribute
    {
        private static readonly string[] EmptyArray = new string[0];
        private string[] rolesSplit = EmptyArray;
        private string roles;

        public AuthorizeRoleAttribute(string roles)
        {
            this.Roles = roles;
        }

        private new string Roles
        {
            get
            {
                return this.roles ?? string.Empty;
            }
            set
            {
                this.roles = value;
                this.rolesSplit = SplitString(value);
            }
        }

        private static string[] SplitString(string original)
        {
            if (string.IsNullOrEmpty(original)) return new string[0];
            return original.Split(',').Select(piece => new { piece = piece, trimmed = piece.Trim() }).Where(s => !string.IsNullOrEmpty(s.trimmed)).Select(param0 => param0.trimmed).ToArray();
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

            IPrincipal principal = actionContext.ControllerContext.RequestContext.Principal;
            if (IsAuthenticated(principal))
            {
                if (this.rolesSplit.Length > 0 && !this.rolesSplit.Any(principal.IsInRole))
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
    }
}