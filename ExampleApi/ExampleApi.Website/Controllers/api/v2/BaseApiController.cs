using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExampleApi.Website.ActionFilters;
using ExampleApi.Website.Models.v2;
using Swashbuckle.Swagger.Annotations;

namespace ExampleApi.Website.Controllers.api.v2
{
    [WebApiAuthorize]
    [SwaggerResponse(HttpStatusCode.Unauthorized, "Either no token or an invalid token was provided", Type = typeof(RequestErrors))]
    [SwaggerResponse(HttpStatusCode.Forbidden, "The token was valid, but does not have access to perform this request", Type = typeof(RequestErrors))]
    public abstract class BaseApiController<T> : ApiController
    {

        private const  Type TypeOfT = typeof (T);
        protected BaseApiController(Permission readOnlyPermission, Permission writeOnlyPermission)
        {
            ReadOnlyPermission = readOnlyPermission;
            WriteOnlyPermission = writeOnlyPermission;
        }

         public Permission ReadOnlyPermission { get; private set; }
         public Permission WriteOnlyPermission { get; private set; }

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <param name="user">The user to create. Note: User.Id will be ignored, and instead auto-incremented.</param>
        /// <returns>The newly created user.</returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = TypeOfT)]
        [SwaggerResponse(422, "The request had improper data", Type = typeof (RequestErrors))]
        public abstract HttpResponseMessage Post(T user);
    }
}