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
    [Route("api/v2/apps/{id?}", Name = "v2-apps")]
    [WebApiAuthorize(Permission.AppsRead, Permission.AppsWrite)]
    [SwaggerResponse(HttpStatusCode.Unauthorized, "Either no token or an invalid token was provided", Type = typeof(RequestErrors))]
    [SwaggerResponse(HttpStatusCode.Forbidden, "The token was valid, but does not have access to perform this request", Type = typeof(RequestErrors))]
    public class AppV2Controller : ApiController
    {
        /// <summary>
        /// Gets all apps, optionally paged.
        /// </summary>
        /// <param name="page">If provided, returns only a subset of users, based on page and itemsPerPage.</param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(App[]))]
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
        /// Gets a single app by id.
        /// </summary>
        /// <param name="id">The id of the app to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(App))]
        [SwaggerResponse(HttpStatusCode.NotFound, "The user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The id provided was not a valid int", Type = typeof(RequestErrors))]
        [Route("api/v2/apps/{id}")]
        public HttpResponseMessage Get(int? id)
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
        /// Patches simple properties of an app. 
        /// This excludes complex properties, such as Users -- those must be managed at /api/{version}/apps/{id}/users.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patches"></param>
        /// <returns></returns>
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        [SwaggerResponse(HttpStatusCode.NotFound, "The user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The request provided was not valid", Type = typeof(RequestErrors))]
        [Route("api/v2/apps/{id}")]
        public HttpResponseMessage Patch(int? id, Patch[] patches)
        {
            throw new NotImplementedException();
        }
    }
}
