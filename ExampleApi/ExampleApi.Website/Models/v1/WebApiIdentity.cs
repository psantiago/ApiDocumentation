using System.Collections.Generic;

namespace ExampleApi.Website.Models.v1
{
    public class WebApiIdentity
    {
        public string Token { get; set; }

        public HashSet<int> AppIds { get; set; }

        public HashSet<Permission> Permissions { get; set; }
    }

    public enum Permission
    {
        Unknown = 0,
        UsersRead,
        UsersWrite
    }
}