using System;
using System.Collections.Generic;
using System.Linq;
using Assets.User;
using DefaultDatabase;
using Redis;
using StackExchange.Redis;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
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

                var loginUser = JsonConvert.DeserializeObject<LoginUserParam>(valueMessage.Value);
                var identity = GetIdentity(loginUser);
                if (identity == null) return;

                var token = GetToken(identity);
                Console.WriteLine($"Token was created for {loginUser.Email}: " + token);
                
                RedisCache.StringSet(loginUser.Email, token);
            });

            Console.WriteLine("Auth component is ready!");
            Console.ReadLine();
        }

        private static IConfigurationRoot Config => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build();

        private static string GetToken(ClaimsIdentity identity)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Config.GetSection("Jwt:Key").Value));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var lifeTime = Convert.ToDouble(Config.GetSection("Jwt:Lifetime").Value);

            var jwt = new JwtSecurityToken(
                Config.GetSection("Jwt:Issuer").Value,
                Config.GetSection("Jwt:Audience").Value,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(lifeTime)),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private static ClaimsIdentity GetIdentity(LoginUserParam loginUserParam)
        {
            using (var db = new ApplicationContext())
            {
                var userData = (from user in db.Users
                    join role in db.Roles on user.RoleId equals role.Id
                    where user.Email == loginUserParam.Email && user.Password == loginUserParam.Password
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