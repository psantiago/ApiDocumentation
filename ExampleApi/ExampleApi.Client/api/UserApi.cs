using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ExampleApi.Client.Models;
using Newtonsoft.Json;
using RestSharp;

namespace ExampleApi.Client.api
{
    public static class UserApi
    {
        //This should technically use int id - it's string just to be able to demonstrate a 422 error.
        public static Task<User> GetAsync(string id)
        {
            return ApiClient.GetAsync<User>("users/" + id);
        }
    }
}
