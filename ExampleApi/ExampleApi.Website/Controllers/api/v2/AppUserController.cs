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
    [Route("api/v2/apps/{appid}/users/{id?}", Name = "v2-apps-users")]
    [WebApiAuthorize(Permission.AppsRead, Permission.AppsWrite)]
    [SwaggerResponse(HttpStatusCode.Unauthorized, "Either no token or an invalid token was provided", Type = typeof(RequestErrors))]
    [SwaggerResponse(HttpStatusCode.Forbidden, "The token was valid, but does not have access to perform this request", Type = typeof(RequestErrors))]
    public class AppUserV2Controller : ApiController
    {
        /// <summary>
        /// Gets all users in the given app, optionally paged.
        /// </summary>
        /// <param name="appid">The id of the app to retrieve.</param>
        /// <param name="page">If provided, returns only a subset of users, based on page and itemsPerPage.</param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Paged<AppUser>))]
        [SwaggerResponse(HttpStatusCode.Found, "For an out of bounds page, you will be redirected to the last available page.")]
        [SwaggerResponse(HttpStatusCode.NotFound, "The app could not be found", Type = typeof(RequestErrors))]
        public HttpResponseMessage Get(int appid, int? page = null, int? itemsPerPage = 25)
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
        /// Gets an app-user resource by app id and user id.
        /// </summary>
        /// <param name="appid">The id of the app to retrieve.</param>
        /// <param name="id">The id of the user of the app-user resource to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AppUser))]
        [SwaggerResponse(HttpStatusCode.NotFound, "The app or user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The appid or user id provided was not a valid int", Type = typeof(RequestErrors))]
        public HttpResponseMessage Get(int appid, int? id)
        {
            if (id == null)
            {
                return Request.CreateResponse((HttpStatusCode)422, new RequestErrors(new Error("Invalid id", "id")));
            }

            var user = UserService.Get(id.Value);

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new RequestErrors(new Error("No user exists with this ID", "id")));
            }

            return Request.CreateResponse(user);
        }

        /// <summary>
        /// Creates or updates an app-user resource.
        /// </summary>
        /// <param name="appid">The id of the app.</param>
        /// <param name="id">The id of the user.</param>
        /// <param name="user">The updated user. Note: AppUser.AppId AppUser.UserId will be ignored and replaced by the url route parameters.</param>
        /// <returns>The updated app-user resource.</returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AppUser))]
        [SwaggerResponse(422, "The request had improper data", Type = typeof(RequestErrors))]
        [SwaggerResponse(HttpStatusCode.NotFound, "The user or app could not be found", Type = typeof(RequestErrors))]
        public HttpResponseMessage Put(int appid, int id, AppUser user)
        {
            throw new NotImplementedException();
            //user.Id = id;
            //try
            //{
            //    return Request.CreateResponse(UserService.Update(user));
            //}
            //catch (Exception)
            //{
            //    return Request.CreateResponse(HttpStatusCode.NotFound, new RequestErrors(new Error("No user exists with this ID", "id")));
            //}
        }

        /// <summary>
        /// Dissassociates a user by id from the given app.
        /// </summary>
        /// <param name="appid">The id of the app to retrieve.</param>
        /// <param name="id">The id of the user.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.NoContent)]
        public HttpResponseMessage Delete(int appid, int id)
        {
            UserService.Delete(id);

            return Request.CreateResponse(HttpStatusCode.NoContent);
        }
    }
}
