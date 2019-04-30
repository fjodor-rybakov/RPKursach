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

        [FromQuery(Name = "limit")] public int Limit { get; set; }
        [FromQuery(Name = "page")] public int Page { get; set; }

        [HttpGet("products")]
        public ActionResult GetProducts()
        {
            try
            {
                var db = new ApplicationContext();
                var count = db.Products.Count();
                var products = (from product in db.Products
                    join company in db.Companies on product.CompanyId equals company.Id
                    join category in db.Categories on product.CategoryId equals category.Id
                    select new
                    {
                        product.ProductName,
                        company.CompanyName,
                        product.Price,
                        product.Description,
                        category.CategoryName
                    }).Skip(Limit * Page).Take(Limit).ToList();

                return Ok(new {totalCount = count, data = products});
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
                    Description = product.Description,
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

                product.ProductName = productParam.ProductName ?? product.ProductName;
                product.Price = productParam.Price <= 0 ? product.Price : productParam.Price;
                product.Description = productParam.Description ?? product.Description;
                product.CompanyId = productParam.CompanyId <= 0 ? product.CompanyId : productParam.CompanyId;
                product.CategoryId = productParam.CategoryId <= 0 ? product.CategoryId : productParam.CategoryId;

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