using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExampleApi.Website.Models.v2;

namespace ExampleApi.Website.Repositories
{
    public class UserRepository
    {
        private static readonly Dictionary<int, User> FakeDataStore;

        static UserRepository()
        {
            FakeDataStore = new Dictionary<int, User>();

            FakeDataStore[1] = new User
            {
                County = CountyEnum.Monongalia,
                Id = 1,
                Name = "Abraham Lincoln",
                Email = "alincoln@america.gov",
                SessionGuid = Guid.NewGuid()
            };

            FakeDataStore[2] = new User
            {
                County = CountyEnum.Marion,
                Id = 2,
                Name = "George Washington",
                Email = "gwash@america.gov",
                SessionGuid = Guid.NewGuid()
            };

            var ctr = 3;

            while (ctr < 1000)
            {
                FakeDataStore[ctr] = new User
                {
                    County = (CountyEnum)Faker.RandomNumber.Next(0, 3),
                    Id = ctr,
                    Name = Faker.Name.FullName(),
                    Email = Faker.Internet.Email(),
                    SessionGuid = Guid.NewGuid()
                };

                ctr++;
            }
        }

        public static IEnumerable<User> Get()
        {
            return FakeDataStore.Values;
        }

        public static User Get(int id)
        {
            return FakeDataStore.ContainsKey(id) ? FakeDataStore[id] : null;
        }

        public static User Get(string email)
        {
            return FakeDataStore.Values.FirstOrDefault(i => i.Email == email);
        }

        public static User Get(Guid sessionGuid)
        {
            return FakeDataStore.Values.FirstOrDefault(i => i.SessionGuid.Equals(sessionGuid));
        }

        public static User Create(User user)
        {
            user.Id = FakeDataStore.Keys.OrderBy(i => i).Last() + 1;
            FakeDataStore[user.Id] = user;

            return user;
        }
        
        public static User Update(User user)
        {
            if (!FakeDataStore.ContainsKey(user.Id)) throw new InvalidDataException("Cannot updated non-existant user");

            FakeDataStore[user.Id] = user;

            return user;
        }

        public static void Delete(int id)
        {
            FakeDataStore.Remove(id);
        }
    }
}