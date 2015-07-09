using System.Linq;
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
    [Route("api/v2/users/{id?}", Name = "v2-users")]
    [Route("api/latest/users/{id?}", Name = "latest-users")]
    [WebApiAuthorize(Permission.UsersRead, Permission.UsersWrite)]
    [SwaggerResponse(HttpStatusCode.Unauthorized, "Either no token or an invalid token was provided", Type = typeof(RequestErrors))]
    [SwaggerResponse(HttpStatusCode.Forbidden, "The token was valid, but does not have access to perform this request", Type = typeof(RequestErrors))]
    public class UsersV2Controller : ApiController
    {
        /// <summary>
        /// Gets all users, optionally paged.
        /// </summary>
        /// <param name="page">If provided, returns only a subset of users, based on page and itemsPerPage.</param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(User[]))]
        [SwaggerResponse(HttpStatusCode.Found, "For an out of bounds page, you will be redirected to the last available page.")]
        public HttpResponseMessage Get(int? page = null, int? itemsPerPage = 25)
        {
            var users = UserService.Get().ToList();
            var usersCount = users.Count();
            if (page != null && page > 0 && itemsPerPage != null && itemsPerPage > 0)
            {
                //if the page they try to get is too large, redirect to the last available page.
                // ReSharper disable once PossibleLossOfFraction - that's the point!
                var lastAvailablePage = (int)Math.Ceiling((double)usersCount / itemsPerPage.Value);

                if (page > lastAvailablePage)
                {
                    var response = Request.CreateResponse(HttpStatusCode.Found);
                    response.Headers.Location = new Uri(Url.Link("v2-users", new { page = lastAvailablePage, itemsPerPage }));
                    return response;
                }

                users = users.Skip(itemsPerPage.Value * (page.Value - 1)).Take(itemsPerPage.Value).ToList();
            }

            return Request.CreateResponse(users);
        }

        /// <summary>
        /// Gets a single user by id.
        /// </summary>
        /// <param name="id">The id of the user to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse(HttpStatusCode.NotFound, "The user could not be found", Type = typeof(RequestErrors))]
        [Route("api/v2/users/{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            var user = UserService.Get(id);

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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse(HttpStatusCode.NotFound, "The user could not be found", Type = typeof(RequestErrors))]
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
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse(HttpStatusCode.NotFound, "The user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The value provided was not a valid email", Type = typeof(RequestErrors))]
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

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="user">The user to create. Note: User.Id will be ignored, and instead auto-incremented.</param>
        /// <returns>The newly created user.</returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse(422, "The request had improper data", Type = typeof(RequestErrors))]
        public HttpResponseMessage Post(User user)
        {
            return Request.CreateResponse(UserService.Create(user));
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="id">The id of the user to update.</param>
        /// <param name="user">The updated user. Note: User.Id will be ignored and replaced by the url route id.</param>
        /// <returns>The updated user.</returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(User))]
        [SwaggerResponse(422, "The request had improper data", Type = typeof(RequestErrors))]
        [SwaggerResponse(HttpStatusCode.NotFound, "The user could not be found", Type = typeof(RequestErrors))]
        public HttpResponseMessage Put(int id, User user)
        {
            user.Id = id;
            try
            {
                return Request.CreateResponse(UserService.Update(user));
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new RequestErrors(new Error("No user exists with this ID", "id")));
            }
        }

        /// <summary>
        /// Deletes a user by id.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public HttpResponseMessage Delete(int id)
        {
            UserService.Delete(id);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
