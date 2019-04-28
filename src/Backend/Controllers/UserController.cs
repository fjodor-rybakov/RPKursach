using System;
using System.Linq;
using System.Threading;
using ApiErrors;
using Assets.User;
using EntityDatabase;
using Microsoft.AspNetCore.Mvc;
using Redis;
using StackExchange.Redis;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly RedisContext Instance = RedisContext.GetInstance();
        private static readonly IDatabase RedisCache = Instance.RedisCache;
        private static readonly ISubscriber Sub = RedisCache.Multiplexer.GetSubscriber();
        private readonly ApiError _apiError = new ApiError();
        private const int CountAttempt = 3;
        private const int TimeMsAttempt = 500;

        [HttpPost("registration")]
        public ActionResult<string> Registration([FromBody] RegistrationUser value)
        {
            // TODO: добавить валидацию

            Sub.Publish(
                RedisEvents.Events.ChannelName,
                RedisContext.CreateMessage(RedisEvents.Events.RegistrationUserEvent, value)
            );
            using (var db = new ApplicationContext())
            {
                var userData = db.Users.Where(u => u.Email == value.Email).ToList();
                if (userData.Count != 0) return _apiError.UserAlreadyExist;

                return RetryGetEmail(db, value.Email)
                    ? _apiError.ServerError
                    : Ok(new {Message = "Пользователь успешно создан"});
            }
        }

        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] LoginUser value)
        {
            // TODO: добавить валидацию

            Sub.Publish(
                RedisEvents.Events.ChannelName,
                RedisContext.CreateMessage(RedisEvents.Events.LoginUserEvent, value)
            );
            var token = RetryGetToken(value.Email);
            RedisCache.KeyDelete(value.Email);

            return token == null ? _apiError.UserNotFount : Ok(new {AccessToken = token});
        }

        private static string RetryGetToken(string email)
        {
            for (var i = 0; i < CountAttempt + 5; i++)
            {
                Thread.Sleep(TimeMsAttempt);
                string dataToken = RedisCache.StringGet(email);
                if (dataToken != null) return dataToken;
            }

            return null;
        }

        private static bool RetryGetEmail(ApplicationContext db, string email)
        {
            for (var i = 0; i < CountAttempt; i++)
            {
                Thread.Sleep(TimeMsAttempt);
                var userData = db.Users.Where(u => u.Email == email).ToList();
                if (userData.Count != 0) return false;
            }

            return true;
        }
    }
}