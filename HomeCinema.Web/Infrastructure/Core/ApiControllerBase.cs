using System;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

using Tracing;

namespace HomeCinema.Web.Infrastructure.Core
{
    public class ApiControllerBase : ApiController
    {
        protected readonly ITracer Tracer;
        public readonly IUnitOfWork _unitOfWork;

        [Obsolete]
        public ApiControllerBase(IEntityBaseRepository<Error> errorsRepository, IUnitOfWork unitOfWork)
        {
        }

        public ApiControllerBase(ITracer tracer)
        {
            this.Tracer = tracer;
        }

        /// <summary>Returns the current ClaimsPrincipal associated with this request.</summary>
        /// <returns>The current ClaimsPrincipal associated with this request.</returns>
        public new ClaimsPrincipal User
        {
            get
            {
                return this.RequestContext.Principal as ClaimsPrincipal;
            }
            set
            {
                this.RequestContext.Principal = value;
            }
        }

        protected bool IsCurrentUser(User user)
        {
            return user.Username == this.User.Identity.Name;
        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response;

            try
            {
                response = function();
            }
            catch (DbUpdateException ex)
            {
                this.Tracer.Exception(ex);

                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                this.Tracer.Exception(ex);

                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }
    }
}