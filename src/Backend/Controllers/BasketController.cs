using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ApiErrors;
using Assets.Basket;
using Assets.User;
using Backend.ShowcaseRequests;
using DefaultDatabase;
using DefaultDatabase.Models;
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
                var product = Transport.GetProductInfo(basketProductParams.ProductId);
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
                    let product = Transport.GetProductInfo(userProduct.ProductId)
                    where userProduct.UserId == user.Id
                    select new
                    {
                        product.ProductName,
                        product.CompanyName,
                        product.Price,
                        product.Description,
                        userProduct.ProductCount,
                        product.CategoryName,
                        product.Image
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
                var token = HttpContext.Request.Headers["Authorization"];

                var db = new ApplicationContext();
                foreach (var item in basketProductParams)
                {
                    var userProducts =
                        db.UserProducts.FirstOrDefault(up => up.UserId == user.Id && up.ProductId == item.ProductId);
                    if (userProducts == null) return _apiError.ProductNotFound;

                    var product = Transport.GetProductInfo(item.ProductId);
                    if (product == null) return _apiError.ProductNotFound;

                    if (product.Count - item.ProductId < 0) return _apiError.IncorrectProductCount;

                    Transport.UpdateProductCount(item.ProductId, product.Count -= item.ProductCount, token);
                    
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