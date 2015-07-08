using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleApi.Client.api;

namespace ExampleApi.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Task.Run(async () =>
                {
                    // var users = await UserApi.BrokenGetAsync();
                    // System.Console.Write(users);
                    try
                    {
                        var user = await UserApi.GetAsyncHttpClient("ham");

                        System.Console.Write(user);
                    }
                    catch (Exception e)
                    {

                        throw;
                    }

                }).Wait();
            }
            catch (Exception)
            {
                
                throw;
            }
           
        }
    }
}
