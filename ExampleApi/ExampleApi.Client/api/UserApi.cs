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
        private static readonly RestClient Client = new RestClient("http://localhost:20760/api/v1");

        public static Task<List<User>> GetAsync()
        {
            return new RestClient("http://localhost:20760/api/v1/").GetTaskAsync<List<User>>(new RestRequest("users", Method.GET));
        }

        public static List<User> Get()
        {
            return Client.Get<List<User>>(new RestRequest("users")).Data;
        }

        public static Task<User> GetAsync(int? id)
        {
            var request = new RestRequest("users", Method.GET);
            request.AddParameter("id", id);
            return Client.GetTaskAsync<User>(request);
        }
        
        public static Task<User> GetAsyncHttpClient(string id)
        {
            return ApiClient.GetAsync<User>("users/" + id);
        }

        public static Task<User> BrokenGetAsync()
        {
            var request = new RestRequest("users", Method.GET);
            request.AddParameter("id", "ham");
            return Client.GetTaskAsync<User>(request);
        }

        public static User BrokenGet()
        {
            var request = new RestRequest("users", Method.GET);
            request.AddParameter("id", "ham");
            var response = Client.Get<User>(request);

            if ((int)response.StatusCode >= 400)
            {
                throw new Exception(response.StatusCode + ":" + response.Content);
            }

            return response.Data;
        }
    }
}
