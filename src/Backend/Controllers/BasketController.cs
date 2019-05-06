using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ApiErrors;
using Assets.Basket;
using Assets.User;
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

        [Authorize(Roles = ERoles.Customer + ", " + ERoles.Administrator)]
        [HttpPost("products")]
        public ActionResult<string> AddBasket([FromBody] BasketProductParams basketProductParams)
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                var db = new ApplicationContext();
                var product = db.Products.FirstOrDefault(p => p.Id == basketProductParams.ProductId);
                if (product == null) return _apiError.ProductNotFound;

                if (product.Count - basketProductParams.ProductCount < 0) return _apiError.IncorrectProductCount;

                db.UserProducts.Add(new UserProduct
                {
                    ProductId = basketProductParams.ProductId,
                    UserId = user.Id,
                    ProductCount = basketProductParams.ProductCount
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

        [Authorize(Roles = ERoles.Customer + ", " + ERoles.Administrator)]
        [HttpGet("products")]
        public ActionResult<string> GetAllBasketProducts()
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                var db = new ApplicationContext();
                var products = (from userProduct in db.UserProducts
                    join product in db.Products on userProduct.ProductId equals product.Id
                    join company in db.Companies on product.CompanyId equals company.Id
                    join category in db.Categories on product.CategoryId equals category.Id
                    where userProduct.UserId == user.Id
                    select new
                    {
                        product.ProductName,
                        company.CompanyName,
                        product.Price,
                        product.Description,
                        userProduct.ProductCount,
                        category.CategoryName,
                        Image = GetBase64(product.ImagePath)
                    }).ToList();

                return Ok(products);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = ERoles.Customer + ", " + ERoles.Administrator)]
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

        [Authorize(Roles = ERoles.Customer + ", " + ERoles.Administrator)]
        [HttpPost("buy")]
        public ActionResult<string> BuyProducts([FromBody] List<BasketProductParams> basketProductParams)
        {
            try
            {
                var user = GetUser(HttpContext.User);
                if (user == null) return _apiError.UserNotFount;

                var db = new ApplicationContext();
                foreach (var item in basketProductParams)
                {
                    var userProducts =
                        db.UserProducts.FirstOrDefault(up => up.UserId == user.Id && up.ProductId == item.ProductId);
                    if (userProducts == null) return _apiError.ProductNotFound;

                    var product = db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null) return _apiError.ProductNotFound;

                    if (product.Count - item.ProductId < 0) return _apiError.IncorrectProductCount;

                    // снятие денег с карты

                    db.PurchaseHistories.Add(new PurchaseHistory
                    {
                        ProductId = item.ProductId,
                        UserId = user.Id,
                        ProductCount = item.ProductCount
                    });
                    db.SaveChanges();
                }

                return Ok(new {Message = "Продукты успешно куплены"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        private static string GetBase64(string imagePath)
        {
            if (!System.IO.File.Exists(imagePath)) return "";
            var bytes = System.IO.File.ReadAllBytes(imagePath);
            var items = imagePath.Split(".");
            var type = items[items.Length - 1];
            return $"data:image/{type};base64, {Convert.ToBase64String(bytes)}";
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