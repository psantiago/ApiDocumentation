using System;
using System.Collections.Generic;
using ExampleApi.Website.Models.v2;
using ExampleApi.Website.Repositories;

namespace ExampleApi.Website.Services
{
    public static class UserService
    {

        public static IEnumerable<User> Get()
        {
            return UserRepository.Get();
        }

        public static User Get(int id)
        {
            return UserRepository.Get(id);
        }

        public static User Get(string email)
        {
            return UserRepository.Get(email);
        }

        public static User Get(Guid sessionGuid)
        {
            return UserRepository.Get(sessionGuid);
        }
    }
}