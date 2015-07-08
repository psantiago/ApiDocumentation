using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExampleApi.Website.ActionFilters;
using ExampleApi.Website.Models.v2;
using ExampleApi.Website.Services;
using Swashbuckle.Swagger.Annotations;
using System;

namespace ExampleApi.Website.Controllers.api.v2
{
    /// <summary>
    /// Compared to version 1, this uses authentication.
    /// </summary>
    [Route("api/v2/users/{id?}")]
    [Route("api/latest/users/{id?}")]
    [WebApiAuthorize(Permission.UsersRead, Permission.UsersWrite)]
    [SwaggerResponse(401, "Either no token or an invalid token was provided", Type = typeof(RequestErrors))]
    [SwaggerResponse(403, "The token was valid, but does not have access to perform this request", Type = typeof(RequestErrors))]
    public class UsersV2Controller : ApiController
    {
        // GET: api/Users
        [SwaggerResponse(200, Type = typeof(User[]))]
        public HttpResponseMessage Get(DateTime? startDate = null, DateTime? endDate = null, int? page = null, int? itemsPerPage = null)
        {
            return  Request.CreateResponse(UserService.Get());
        }

        /// <summary>
        /// Gets a single user by id.
        /// </summary>
        /// <param name="id">The id of the user to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(200, Type = typeof(User))]
        [SwaggerResponse(404, "The user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The id provided was not a valid int", Type = typeof(RequestErrors))]
        [Route("api/v2/users/{id:int}")]
        public HttpResponseMessage Get(int? id)
        {
            if (id == null)
            {
                return Request.CreateResponse((HttpStatusCode)422, new RequestErrors(new Error("Invalid ID", "id")));
            }

            var user = UserService.Get(id.Value);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new RequestErrors(new Error("No user exists with this ID", "id")));
            }

            return Request.CreateResponse(user);
        }
        
        /// <summary>
        /// Gets a single user by session guid.
        /// </summary>
        /// <param name="sessionGuid">The session guid of the user to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(200, Type = typeof(User))]
        [SwaggerResponse(404, "The user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The id provided was not a valid int", Type = typeof(RequestErrors))]
        [Route(@"api/v2/users/{sessionGuid:guid}")]
        public HttpResponseMessage Get(Guid sessionGuid)
        {
            var user = UserService.Get(sessionGuid);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new RequestErrors(new Error("No user exists with this ID", "id")));
            }

            return Request.CreateResponse(user);
        }


        /// <summary>
        /// Gets a single user by email.
        /// </summary>
        /// <param name="name">The email of the user to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(200, Type = typeof(User))]
        [SwaggerResponse(404, "The user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The id provided was not a valid email", Type = typeof(RequestErrors))]
        [Route(@"api/v2/users/{email:regex(^[^\d]+$)}")]
        public HttpResponseMessage Get(string email)
        {
            if (email == null)
            {
                return Request.CreateResponse((HttpStatusCode)422, new RequestErrors(new Error("Invalid name", "name")));
            }

            var user = UserService.Get(email);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new RequestErrors(new Error("No user exists with this ID", "id")));
            }

            return Request.CreateResponse(user);
        }

        // POST: api/Users
        public void Post(User user)
        {
        }

        // PUT: api/Users/5
        public void Put(int id, User user)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }
    }
}
