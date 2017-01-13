using System;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Entities;

namespace HomeCinema.Web.Infrastructure.Core
{
    public class ApiControllerBase : ApiController
    {
        protected readonly IEntityBaseRepository<Error> _errorsRepository;
        protected readonly IUnitOfWork _unitOfWork;

        public ApiControllerBase(IEntityBaseRepository<Error> errorsRepository, IUnitOfWork unitOfWork)
        {
            this._errorsRepository = errorsRepository;
            this._unitOfWork = unitOfWork;
        }

        public ApiControllerBase(IDataRepositoryFactory dataRepositoryFactory, IEntityBaseRepository<Error> errorsRepository, IUnitOfWork unitOfWork)
        {
            this._errorsRepository = errorsRepository;
            this._unitOfWork = unitOfWork;
        }

        protected HttpResponseMessage CreateHttpResponse(HttpRequestMessage request, Func<HttpResponseMessage> function)
        {
            HttpResponseMessage response = null;

            try
            {
                response = function.Invoke();
            }
            catch (DbUpdateException ex)
            {
                this.LogError(ex);
                response = request.CreateResponse(HttpStatusCode.BadRequest, ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                response = request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }

            return response;
        }

        private void LogError(Exception ex)
        {
            try
            {
                var error = new Error { Message = ex.Message, StackTrace = ex.StackTrace, DateCreated = DateTime.Now };

                this._errorsRepository.Add(error);
                this._unitOfWork.Commit();
            }
            catch
            {
            }
        }
    }
}