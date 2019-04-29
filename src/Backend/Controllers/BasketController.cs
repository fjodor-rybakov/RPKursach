using System;
using System.Linq;
using System.Security.Claims;
using ApiErrors;
using EntityDatabase;
using EntityDatabase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly ApiError _apiError = new ApiError();

        [Authorize(Roles = "Покупатель")]
        [HttpGet("{id}")]
        public ActionResult<string> AddBasket(int id)
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                var db = new ApplicationContext();
                db.UserProducts.Add(new UserProduct
                {
                    ProductId = id,
                    UserId = user.Id
                });
                db.SaveChanges();

                return Ok(new {Message = "Продукт успещно добавлен в конзину"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = "Покупатель")]
        [HttpGet("products")]
        public ActionResult<string> GetAllBasketProducts()
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;
                
                var db = new ApplicationContext();
                var products =  (from userProduct in db.UserProducts 
                    join product in db.Products on userProduct.ProductId equals product.Id
                    where userProduct.UserId == user.Id
                    select new
                    {
                        product.ProductName,
                        product.Price
                    }).ToList();

                return Ok(products);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = "Покупатель")]
        [HttpDelete("{id}")]
        public ActionResult<string> DeleteBasketProduct(int id)
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                var db = new ApplicationContext();
                var userProducts = db.UserProducts.FirstOrDefault(up => up.UserId == user.Id && up.ProductId == id);
                if (userProducts == null) return _apiError.ProductNotFound;
                
                db.UserProducts.Remove(userProducts);
                db.SaveChanges();
                
                return Ok(new {Message = "Продукт успешно удалён из корзины"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
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