using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExampleApi.Website.Models.v1;
using Swashbuckle.Swagger.Annotations;

namespace ExampleApi.Website.Controllers.api.v1
{
    [Route("api/v1/users/{id?}")]
    public class UsersController : ApiController
    {
        // GET: api/Users
        [SwaggerResponse(200, Type = typeof(User[]))]
        public HttpResponseMessage Get()
        {
            return  Request.CreateResponse(new[] {
                new User
                {
                    County = CountyEnum.Monongalia,
                    Id = 1,
                    Name = "Abraham Lincoln"
                },
                new User
                {
                    County = CountyEnum.Marion,
                    Id = 2,
                    Name = "George Washington"
                }
            });
        }

        /// <summary>
        /// Gets a single user by id.
        /// </summary>
        /// <param name="id">The id of the user to retrieve.</param>
        /// <returns></returns>
        [SwaggerResponse(200, Type = typeof(User))]
        [SwaggerResponse(404, "The user could not be found", Type = typeof(RequestErrors))]
        [SwaggerResponse(422, "The id provided was not a valid int", Type = typeof(RequestErrors))]
        public HttpResponseMessage Get(int? id)
        {
            if (id == null)
            {
                return Request.CreateResponse((HttpStatusCode)422, new RequestErrors(new Error("Invalid ID", "id")));
            }

            if (id > 10)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new RequestErrors(new Error("Invalid ID", "id")));
            }

            return Request.CreateResponse(new User
            {
                County = CountyEnum.Monongalia,
                Id = id.Value,
                Name = "Abraham Lincoln"
            });
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
