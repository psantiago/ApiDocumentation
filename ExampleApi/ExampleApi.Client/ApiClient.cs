using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExampleApi.Client
{
    internal static class ApiClient
    {
        private const string BaseUrl = "http://localhost:20760/api/v1/";
        private const string Token = "123456";

        private static readonly HttpClient Client;

        static ApiClient()
        {
            Client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }

        public async static Task<T> GetAsync<T>(string partialUrl)
        {
            return await ProcessResponse<T>(await Client.GetAsync(partialUrl));
        }

        private async static Task<T> ProcessResponse<T>(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            try
            {
                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<T>(body);
            }
            catch (HttpRequestException e)
            {
                throw new Exception(e.Message, new HttpRequestException(body));
            }
        }
    }
}