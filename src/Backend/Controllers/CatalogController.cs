using System;
using System.Linq;
using EntityDatabase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        [HttpGet("products")]
        public ActionResult GetProducts()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                /*var products = db.Products
                    .Join(db.Companies,
                        p => p.CompanyId,
                        c => c.Id,
                        (p, c) => new
                        {
                            p.ProductName,
                            c.CompanyName,
                            p.Price
                        })
                    .ToList();*/
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
        }
    }
}