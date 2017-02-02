using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    [AuthorizeRole(Roles.SystemAdmin)]
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

        [HttpPost]
        //[AuthorizeClaim("user.add")]
        [Route("add")]
        public HttpResponseMessage Add(HttpRequestMessage request, UserDetailViewModel userDetailViewModel)
        {
            return this.CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!this.ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
                }
                else
                {
                    // TODO IMplement Add New user
                    // TODO Check if user already exists? Reuse registration logic?
                    var isExistingUser = this.membershipService.IsExistingUser(userDetailViewModel.Id);
                    if (isExistingUser)
                    {
                        return request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    var newUser = Mapper.Map<UserDetailViewModel, User>(userDetailViewModel);
                    var roles = Mapper.Map<IEnumerable<RoleViewModel>, IEnumerable<Role>>(userDetailViewModel.Roles);
                    var addedUser = this.membershipService.CreateUser(newUser.Username, "TODO GATH: Not possible", "", roles.ToArray()); //TODO: Not possible

                    var viewModel = Mapper.Map<User, UserDetailViewModel>(addedUser);
                    response = request.CreateResponse(HttpStatusCode.Created, viewModel);
                }

                return response;
            });
        }

        [HttpPost]
        //[Authenticate]
        [Route("update")]
        public HttpResponseMessage Update(HttpRequestMessage request, UserDetailViewModel userDetailViewModel)
        {
            return this.CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!this.ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
                }
                else
                {
                    var user = this.membershipService.GetUser(userDetailViewModel.Id);
                    if (user == null)
                    {
                        return request.CreateResponse(HttpStatusCode.BadRequest);
                    }

                    var updateUser = Mapper.Map<UserDetailViewModel, User>(userDetailViewModel);
                    this.membershipService.UpdateUser(user, updateUser);

                    var viewModel = Mapper.Map<User, UserDetailViewModel>(user);
                    response = request.CreateResponse(HttpStatusCode.Created, viewModel);
                }

                return response;
            });
        }


        [HttpGet]
        [AuthorizeClaim(Claims.UserAdmin)]
        [Route("delete/{userId:int}")]
        public HttpResponseMessage Delete(HttpRequestMessage request, int userId)
        {
            return this.CreateHttpResponse(request, () =>
            {
                if (!this.ModelState.IsValid)
                {
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
                }

                var user = this.membershipService.GetUser(userId);
                if (user == null)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "User does not exist");
                }

                if (user.IsSystemDefault)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Cannot delete system default user");
                }

                if (this.IsCurrentUser(user))
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest, "Cannot delete your own user");
                }

                this.membershipService.DeleteUser(user);

                return request.CreateResponse(HttpStatusCode.OK, userId);
            });
        }

        //[Authorize]
        [Route("details/{id:int}")]
        [HttpGet]
        public HttpResponseMessage GetUser(HttpRequestMessage request, int id)
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

                    if (this.IsCurrentUser(user) || this.User.HasClaim(c => c.Value == Claims.UserAdmin))
                    {
                        var userDetailViewModel = Mapper.Map<User, UserDetailViewModel>(user);
                        return request.CreateResponse(HttpStatusCode.OK, userDetailViewModel);
                    }

                    return request.CreateResponse(HttpStatusCode.Unauthorized);
                });
        }

        [AuthorizeClaim(Claims.UserAdmin)]
        [Route("role/details/{roleId:int}")]
        [HttpGet]
        public HttpResponseMessage GetRole(HttpRequestMessage request, int roleId)
        {
            return this.CreateHttpResponse(request, () =>
            {
                var role = this.membershipService.GetRole(roleId);
                if (role == null)
                {
                    return request.CreateResponse(HttpStatusCode.BadRequest);
                }

                var roleViewModel = Mapper.Map<Role, RoleViewModel>(role);
                return request.CreateResponse(HttpStatusCode.OK, roleViewModel);
            });
        }

        [AuthorizeClaim(Claims.UserAdmin)]
        [HttpPost]
        [Route("role/update")]
        public HttpResponseMessage UpdateRole(HttpRequestMessage request, RoleViewModel roleViewModel)
        {
            return this.CreateHttpResponse(
                request,
                () =>
                    {
                        var role = this.membershipService.GetRole(roleViewModel.Id);
                        if (role == null)
                        {
                            return request.CreateResponse(HttpStatusCode.BadRequest);
                        }

                        var updateRole = Mapper.Map<RoleViewModel, Role>(roleViewModel);
                        this.membershipService.UpdateRole(role, updateRole);

                        var viewModel = Mapper.Map<Role, RoleViewModel>(role);
                        return request.CreateResponse(HttpStatusCode.Created, viewModel);
                    });
        }

        [AuthorizeClaim(Claims.UserAdmin)]
        [Route("users")]
        [HttpGet]
        public HttpResponseMessage GetAllUsers(HttpRequestMessage request)
        {
            return this.CreateHttpResponse(request, () =>
            {
                var users = this.membershipService.GetUsers();
                var userViewModels = Mapper.Map<IEnumerable<User>, IEnumerable<UserViewModel>>(users);

                //var pagedSet = new PaginationSet<UserViewModel>
                //{
                //    Page = currentPage,
                //    TotalCount = totalMovies,
                //    TotalPages = (int)Math.Ceiling((decimal)totalMovies / currentPageSize),
                //    Items = usersViewModel
                //};

                //response = request.CreateResponse<PaginationSet<MovieViewModel>>(HttpStatusCode.OK, users);

                return request.CreateResponse(HttpStatusCode.OK, userViewModels);
            });
        }

        [AuthorizeClaim(Claims.UserAdmin)]
        [Route("roles")]
        [HttpGet]
        public HttpResponseMessage GetAllRoles(HttpRequestMessage request)
        {
            return this.CreateHttpResponse(request, () =>
            {
                var roles = this.membershipService.GetRoles();
                var roleViewModels = Mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(roles);

                return request.CreateResponse(HttpStatusCode.OK, roleViewModels);
            });
        }

        [AuthorizeClaim(Claims.UserAdmin)]
        [Route("claims")]
        [HttpGet]
        public HttpResponseMessage GetAllClaims(HttpRequestMessage request)
        {
            return this.CreateHttpResponse(request, () =>
            {
                var claims = this.membershipService.GetClaims();
                var claimViewModels = Mapper.Map<IEnumerable<Claim>, IEnumerable<ClaimViewModel>>(claims);
                foreach (var claimViewModel in claimViewModels)
                {
                    var roles = this.membershipService.GetRolesByClaimId(claimViewModel.Id);
                    var roleViewModels = Mapper.Map<IEnumerable<Role>, IEnumerable<RoleViewModel>>(roles);
                    claimViewModel.Roles = roleViewModels.ToList();
                }

                return request.CreateResponse(HttpStatusCode.OK, claimViewModels);
            });
        }

        [HttpPost]
        [AuthorizeClaim(Claims.UserAdmin)]
        [Route("claims/update")]
        public HttpResponseMessage Update(HttpRequestMessage request, IEnumerable<ClaimViewModel> claimViewModels)
        {
            return this.CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;

                if (!this.ModelState.IsValid)
                {
                    response = request.CreateErrorResponse(HttpStatusCode.BadRequest, this.ModelState);
                }
                else
                {

                    response = request.CreateResponse(HttpStatusCode.OK);
                }

                return response;
            });
        }

    }
}
