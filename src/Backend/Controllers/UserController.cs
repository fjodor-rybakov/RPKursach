using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using ApiErrors;
using Assets.User;
using EntityDatabase;
using EntityDatabase.Models;
using Microsoft.AspNetCore.Authorization;
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
        private const int CountAttempt = 10;
        private const int TimeMsAttempt = 500;

        [HttpPost("registration")]
        public ActionResult<string> Registration([FromBody] RegistrationUserParam registrationUser)
        {
            try
            {
                var db = new ApplicationContext();
                var userData = db.Users.Where(u => u.Email == registrationUser.Email).ToList();
                if (userData.Count != 0) return _apiError.UserAlreadyExist;

                var user = new User
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

                return Ok(new {Message = "Пользователь успешно создан"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [HttpPost("login")]
        public ActionResult<string> Login([FromBody] LoginUserParam loginUserParam)
        {
            try
            {
                var db = new ApplicationContext();
                var userData = db.Users.FirstOrDefault(u => u.Email == loginUserParam.Email);
                if (userData == null) return _apiError.UserNotFount;

                Sub.Publish(
                    RedisEvents.Events.ChannelName,
                    RedisContext.CreateMessage(RedisEvents.Events.LoginUserEvent, loginUserParam)
                );
                var token = RetryGetToken(loginUserParam.Email);
                RedisCache.KeyDelete(loginUserParam.Email);

                return token == null ? _apiError.UserNotFount : Ok(new {AccessToken = token});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = ERoles.Customer)]
        [HttpGet]
        public ActionResult<string> GetUserInfo()
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                return Ok(new
                {
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.PaymentCard
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = ERoles.Customer)]
        [HttpGet("history")]
        public ActionResult<string> GetPurchaseHistory()
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                var db = new ApplicationContext();
                var userPurchaseHistory = (from purchaseHistory in db.PurchaseHistories
                    join product in db.Products on purchaseHistory.ProductId equals product.Id
                    where purchaseHistory.UserId == user.Id
                    select new
                    {
                        purchaseHistory.ProductCount,
                        product.Price,
                        product.ProductName,
                        product.Description
                    }).ToList();

                return Ok(userPurchaseHistory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = ERoles.Administrator)]
        [HttpGet("history/all")]
        public ActionResult<string> GetPurchaseHistoryAll()
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                var db = new ApplicationContext();
                var userPurchaseHistory = (from purchaseHistory in db.PurchaseHistories
                    join product in db.Products on purchaseHistory.ProductId equals product.Id
                    select new
                    {
                        purchaseHistory.ProductCount,
                        product.Price,
                        product.ProductName,
                        product.Description,
                        purchaseHistory.ProductId,
                        purchaseHistory.UserId
                    }).ToList();

                return Ok(userPurchaseHistory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        private static string RetryGetToken(string email)
        {
            for (var i = 0; i < CountAttempt; i++)
            {
                Thread.Sleep(TimeMsAttempt);
                string dataToken = RedisCache.StringGet(email);
                if (dataToken != null) return dataToken;
            }

            return null;
        }

        private static User GetUser(ClaimsPrincipal principal)
        {
            using (var db = new ApplicationContext())
            {
                var email = principal.Claims?.First(x => x.Type == ClaimTypes.Name).Value;
                return db.Users.FirstOrDefault(u => u.Email == email);
            }
        }
    }
}