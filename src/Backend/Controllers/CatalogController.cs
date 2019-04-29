using System;
using System.Linq;
using ApiErrors;
using Assets.Catalog;
using EntityDatabase;
using EntityDatabase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ApiError _apiError = new ApiError();
        
        [HttpGet("products")]
        public ActionResult GetProducts()
        {
            try
            {
                var db = new ApplicationContext();
                var products = (from product in db.Products
                    join company in db.Companies on product.CompanyId equals company.Id
                    join category in db.Categories on product.CategoryId equals category.Id
                    select new
                    {
                        product.ProductName,
                        company.CompanyName,
                        product.Price,
                        category.CategoryName
                    }).ToList();
                
                return Ok(products);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost("products")]
        public ActionResult<string> AddProduct([FromBody] ProductParam product)
        {
            try
            {
                var db = new ApplicationContext();
                db.Products.Add(new Product
                {
                    ProductName = product.ProductName,
                    Price = product.Price,
                    CompanyId = product.CompanyId,
                    CategoryId = product.CategoryId
                });

                db.SaveChanges();
                
                return Ok(new {Message = "Продукты успешно добавлены"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = "Администратор")]
        [HttpPut("products/{id}")]
        public ActionResult<string> UpdateProduct(int id, [FromBody] ProductParam productParam)
        {
            try
            {
                var db = new ApplicationContext();
                var product = db.Products.FirstOrDefault(p => p.Id == id);
                if (product == null) return _apiError.ProductNotFound;

                product.ProductName = productParam.ProductName;
                product.Price = productParam.Price;
                product.CompanyId = productParam.CompanyId;
                product.CategoryId = productParam.CategoryId;
                
                db.SaveChanges();
                
                return Ok(new {Message = "Продукт успешно обновлён"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }

        [Authorize(Roles = "Администратор")]
        [HttpDelete("products/{id}")]
        public ActionResult<string> DeleteProduct(int id)
        {
            try
            {
                var db = new ApplicationContext();
                var product = db.Products.FirstOrDefault(p => p.Id == id);
                if (product == null) return _apiError.ProductNotFound;
                
                db.Products.Remove(product);
                db.SaveChanges();
                
                return Ok(new {Message = "Продукт упешно удалён"});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return _apiError.ServerError;
            }
        }
    }
}