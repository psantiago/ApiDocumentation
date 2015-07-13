using System.Collections.Generic;
using ExampleApi.Website.Models.v2;
using ExampleApi.Website.Repositories;

namespace ExampleApi.Website.Services
{
    public class AppService
    {
        public static IEnumerable<App> Get()
        {
            return AppRepository.Get();
        }

        public static App Get(int id)
        {
            return AppRepository.Get(id);
        } 
    }
}