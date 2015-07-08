using System.Collections.Generic;
using ExampleApi.Website.Models.v2;
using ExampleApi.Website.Repositories;

namespace ExampleApi.Website.Services
{
    /// <summary>
    /// Retrieves data from fake data source (that should technically be in an infrastructure project, but whatever.)
    /// </summary>
    public static class WebApiIdentityService
    {
        public static IEnumerable<WebApiIdentity> Get()
        {
            return WebApiIdentityRepository.Get();
        }

        public static WebApiIdentity Get(string token)
        {
            return WebApiIdentityRepository.Get(token);
        }
    }
}