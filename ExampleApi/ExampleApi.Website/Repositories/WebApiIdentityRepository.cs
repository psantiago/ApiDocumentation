using System.Collections.Generic;
using System.Linq;
using ExampleApi.Website.Models.v2;

namespace ExampleApi.Website.Repositories
{
    public static class WebApiIdentityRepository
    {
        private static readonly Dictionary<string, WebApiIdentity> FakeDataStore;

        static WebApiIdentityRepository()
        {
            FakeDataStore = new Dictionary<string, WebApiIdentity>();
            FakeDataStore["readonly"] = new WebApiIdentity
            {
                Token = "readonly",
                AppIds = new HashSet<int>(new[] { 1, 2, 3 }),
                Permissions = new HashSet<Permission>(new[] { Permission.UsersRead })
            };

            FakeDataStore["writeonly"] = new WebApiIdentity
            {
                Token = "writeonly",
                AppIds = new HashSet<int>(new[] { 1, 2, 3 }),
                Permissions = new HashSet<Permission>(new[] { Permission.UsersWrite })
            };

            FakeDataStore["admin"] = new WebApiIdentity
            {
                Token = "admin",
                AppIds = new HashSet<int>(new[] { 1, 2, 3 }),
                Permissions = new HashSet<Permission>(new[] { Permission.UsersWrite, Permission.UsersRead })
            };
        }

        public static IEnumerable<WebApiIdentity> Get()
        {
            return FakeDataStore.Values.ToList();
        }

        public static WebApiIdentity Get(string token)
        {
            return FakeDataStore.ContainsKey(token) ? FakeDataStore[token] : null;
        }
    }
}