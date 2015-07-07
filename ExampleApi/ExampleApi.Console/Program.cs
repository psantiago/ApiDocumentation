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
            var usersTask = UserApi.GetAsync();

            usersTask.Wait();

            var users = usersTask.Result;
        }
    }
}
