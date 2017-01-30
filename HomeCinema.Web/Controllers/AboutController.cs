using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;
using HomeCinema.Web.Infrastructure.Core;

namespace HomeCinema.Web.Controllers
{
    [RoutePrefix("api/about")]
    public class AboutController : ApiControllerBase
    {
        public AboutController(IEntityBaseRepository<Error> _errorsRepository, IUnitOfWork _unitOfWork)
            : base(_errorsRepository, _unitOfWork)
        {
        }

        [HttpGet]
        [Route("")]
        //[AllowAnonymous]
        //[System.Web.Http.Authorize(Roles = "Admin", Users = "1234")]

        //[AuthorizeClaim("about.read", "about.test")]
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            //throw new Exception("test error");

            return this.CreateHttpResponse(
                request,
                () =>
                    {
                        var about = new { Version = "1.0.0.0", MigrationState = "InitialCreate" };

                        return request.CreateResponse(HttpStatusCode.OK, about);
                    });
        }

        [HttpGet]
        [Route("crash")]
        [AllowAnonymous]
        public HttpResponseMessage CrashMethod(HttpRequestMessage request)
        {
            throw new Exception("Something bad happened here.");
        }
    }
}