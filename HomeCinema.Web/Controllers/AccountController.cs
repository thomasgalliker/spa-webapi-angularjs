using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

using AutoMapper;

using Guards;
using HomeCinema.Entities;
using HomeCinema.Services.Abstract;
using HomeCinema.Web.Infrastructure.Core;
using HomeCinema.Web.Models;

using Tracing;

namespace HomeCinema.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiControllerBase
    {
        private readonly IMembershipService membershipService;

        public AccountController(ITracer tracer, IMembershipService membershipService)
            : base(tracer)
        {
            Guard.ArgumentNotNull(membershipService, nameof(membershipService));

            this.membershipService = membershipService;
        }

        [AllowAnonymous]
        [Route("authenticate")]
        [HttpPost]
        public HttpResponseMessage Login(HttpRequestMessage request, LoginViewModel user)
        {
            return this.CreateHttpResponse(request, () =>
                {
                    var loginResultViewModel = new LoginResultViewModel { Success = false, UserId = 0 };

                    if (this.ModelState.IsValid)
                    {
                        var userContext = this.membershipService.ValidateUser(user.Username, user.Password);
                        if (userContext.IsValid())
                        {
                            loginResultViewModel.Success = true;
                            loginResultViewModel.UserId = userContext.User.ID;
                        }
                    }

                    return request.CreateResponse(HttpStatusCode.OK, loginResultViewModel);
                });
        }

        public class LoginResultViewModel
        {
            public bool Success { get; set; }

            public int UserId { get; set; }
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public HttpResponseMessage Register(HttpRequestMessage request, RegistrationViewModel registrationViewModel)
        {
            Guard.ArgumentNotNull(registrationViewModel, nameof(registrationViewModel));

            return this.CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!this.ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, new { success = false });
                }
                else
                {
                    var createdUser = this.membershipService.CreateUser(
                        username: registrationViewModel.Username, 
                        password: registrationViewModel.Password,
                        email: registrationViewModel.Email, 
                        roles: Roles.GuestRole);
                    if (createdUser != null)
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = true });
                    }
                    else
                    {
                        response = request.CreateResponse(HttpStatusCode.OK, new { success = false });
                    }
                }

                return response;
            });
        }

        [AllowAnonymous]
        [Route("details/{id:int}")]
        [HttpGet]
        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            return this.CreateHttpResponse(request, () =>
                {

                    if (!this.ModelState.IsValid)
                    {
                        var response = request.CreateResponse(HttpStatusCode.BadRequest,
                            this.ModelState.Keys
                                .SelectMany(k => this.ModelState[k].Errors)
                                .Select(m => m.ErrorMessage)
                                .ToArray());

                        return response;
                    }

                    var user = this.membershipService.GetUser(id);
                    if (user == null)
                    {
                        return request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    if (user.Username == this.User.Identity.Name || ((ClaimsPrincipal)this.User).HasClaim(c => c.Value == Claims.UserAdmin))
                    {
                        return request.CreateResponse(HttpStatusCode.OK,
                            new
                            {
                                Id = user.ID,
                                Username = user.Username,
                                Email = user.Email,
                                Roles = user.UserRoles.Select(ur => ur.Role.Name),
                                Claims = user.UserRoles.Select(ur => ur.Role).SelectMany(r => r.RoleClaims.Select(rc => rc.Claim.ClaimValue))
                            });
                    }

                    return request.CreateResponse(HttpStatusCode.Unauthorized);
                });
        }

        [AuthorizeClaim(Claims.UserAdmin)]
        [Route("users")]
        [HttpGet]
        public HttpResponseMessage GetAllUsers(HttpRequestMessage request)
        {
            return this.CreateHttpResponse(request, () =>
            {

                if (!this.ModelState.IsValid)
                {
                    var response = request.CreateResponse(HttpStatusCode.BadRequest,
                        this.ModelState.Keys
                            .SelectMany(k => this.ModelState[k].Errors)
                            .Select(m => m.ErrorMessage)
                            .ToArray());

                    return response;
                }

                var users = this.membershipService.GetUsers();
                var usersViewModel = Mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);

                //var pagedSet = new PaginationSet<UserViewModel>
                //{
                //    Page = currentPage,
                //    TotalCount = totalMovies,
                //    TotalPages = (int)Math.Ceiling((decimal)totalMovies / currentPageSize),
                //    Items = usersViewModel
                //};

                //response = request.CreateResponse<PaginationSet<MovieViewModel>>(HttpStatusCode.OK, users);

                return request.CreateResponse(HttpStatusCode.OK, usersViewModel);
            });
        }
    }
}
