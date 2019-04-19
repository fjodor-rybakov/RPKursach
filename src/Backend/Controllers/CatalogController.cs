using System;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        [HttpGet("products")]
        public ActionResult GetProducts()
        {
            return Ok("all products");
        }
    }
}