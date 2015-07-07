using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleApi.Client.Models;
using RestSharp;

namespace ExampleApi.Client.api
{
    public static class UserApi
    {
        private static readonly RestClient Client = new RestClient("http://localhost:20760/api/v1/");

        public static Task<List<User>> GetAsync()
        {
            return Client.GetTaskAsync<List<User>>(new RestRequest("users", Method.GET));
        }
    }
}
