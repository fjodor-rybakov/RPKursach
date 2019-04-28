using System;
using System.Collections.Generic;
using System.Linq;

using Assets.User;
using EntityDatabase;
using Redis;
using StackExchange.Redis;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;

namespace Auth
{
    class Program
    {
        private static readonly RedisContext Instance = RedisContext.GetInstance();
        private static readonly IDatabase RedisCache = Instance.RedisCache;
        private static readonly ISubscriber Sub = RedisCache.Multiplexer.GetSubscriber();

        public static void Main()
        {
            Sub.Subscribe(RedisEvents.Events.ChannelName, (channel, message) =>
            {
                var valueMessage = RedisContext.GetMessage(message);
                if (valueMessage.Event != RedisEvents.Events.LoginUserEvent) return;
                
                var loginUser = JsonConvert.DeserializeObject<LoginUser>(valueMessage.Value);
                var identity = GetIdentity(loginUser);
                if (identity == null) return;
                RedisCache.StringSet(loginUser.Email, GetToken(identity));
            });

            Console.WriteLine("Auth component is ready!");
            Console.ReadLine();
        }

        private static string GetToken(ClaimsIdentity identity)
        {
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                AuthOptions.ISSUER,
                AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static ClaimsIdentity GetIdentity(LoginUser loginUser)
        {
            using (var db = new ApplicationContext())
            {
                var userData = (from user in db.Users
                    join role in db.Roles on user.RoleId equals role.Id
                    where user.Email == loginUser.Email && user.Password == loginUser.Password
                    select new
                    {
                        user.Email,
                        role.RoleName
                    }).FirstOrDefault();

                if (userData == null) return null;

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, userData.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, userData.RoleName)
                };

                return new ClaimsIdentity(
                    claims,
                    "Token",
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType
                );
            }
        }
    }
}