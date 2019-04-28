using System;
using System.Linq;
using EntityDatabase;
using Assets.User;
using Newtonsoft.Json;
using Redis;
using StackExchange.Redis;

namespace User
{
    class Program
    {
        private static readonly RedisContext Instance = RedisContext.GetInstance();
        private static readonly IDatabase RedisCache = Instance.RedisCache;
        private static readonly ISubscriber Sub = RedisCache.Multiplexer.GetSubscriber();

        static void Main()
        {
            Sub.Subscribe(RedisEvents.Events.ChannelName, (channel, message) =>
            {
                var valueMessage = RedisContext.GetMessage(message);
                if (valueMessage.Event != RedisEvents.Events.RegistrationUserEvent) return;

                var newUser = CreateUser(valueMessage.Value);

                Sub.Publish(
                    RedisEvents.Events.ChannelName,
                    RedisContext.CreateMessage(RedisEvents.Events.UserCreatedEvent, newUser)
                );
            });

            Console.WriteLine("User component is ready!");
            Console.ReadLine();
        }

        private static string CreateUser(string value)
        {
            var registrationUser = JsonConvert.DeserializeObject<RegistrationUserParam>(value);
            using (var db = new ApplicationContext())
            {
                var userData = db.Users.Where(u => u.Email == registrationUser.Email).ToList();
                if (userData.Count != 0) return null;

                var user = new EntityDatabase.Models.User
                {
                    Email = registrationUser.Email,
                    Password = registrationUser.Password,
                    FirstName = registrationUser.FirstName,
                    LastName = registrationUser.LastName,
                    PaymentCard = registrationUser.PaymentCard,
                    RoleId = 2
                };
                db.Users.Add(user);
                db.SaveChanges();

                return JsonConvert.SerializeObject(user);
            }
        }
    }
}