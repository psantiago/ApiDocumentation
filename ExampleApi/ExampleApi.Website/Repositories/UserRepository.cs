using System;
using System.Collections.Generic;
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
    }
}