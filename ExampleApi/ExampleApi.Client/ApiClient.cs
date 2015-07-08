using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExampleApi.Client
{
    internal static class ApiClient
    {
        private const string BaseUrl = "http://localhost:20760/api/v1/";

        public async static Task<T> GetAsync<T>(string partialUrl)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                var response = await client.GetAsync(partialUrl);
                var body = await response.Content.ReadAsStringAsync();
                try
                {
                    response.EnsureSuccessStatusCode();
                    return JsonConvert.DeserializeObject<T>(body);
                }
                catch (HttpRequestException e)
                {
                    //e.Data["Body"] = body;
                    //throw;

                    throw new Exception(e.Message, new HttpRequestException(body));
                }
            }
        }
    }
}