using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiErrors;
using Assets.User;
using EntityDatabase;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            
            var json = JsonConvert.SerializeObject(value);
            Sub.Publish(RedisEvents.Events.ChannelName, CreateMessage(json));
            var isError = true;
            using (var db = new ApplicationContext())
            {
                var userData = db.Users.Where(u => u.Email == value.Email).ToList();
                if (userData.Count != 0) return _apiError.UserAlreadyExist;
                
                for (var i = 0; i < CountAttempt; i++)
                {
                    Thread.Sleep(TimeMsAttempt);
                    userData = db.Users.Where(u => u.Email == value.Email).ToList();
                    if (userData.Count != 0) isError = false;
                }
            }
            
            return isError ? _apiError.ServerError : Ok(new {Message = "Пользователь успешно создан"});
        }

        private string CreateMessage(string value)
        {
            return RedisEvents.Events.RegistrationUserEvent + ":" + value;
        }
    }
}