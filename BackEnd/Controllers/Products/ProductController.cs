using BackEnd.DataAccess;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEnd.Controllers.Products
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        public ProductController(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet("[action]")]
        [Authorize(Policy = "IsLoggedIn")]
        public IActionResult GetAllProducts()
        {
            
            return Ok(_db.Products.ToList());
        }

        [HttpGet("[action]/{id}")]
        [Authorize(Policy = "IsLoggedIn")]
        public IActionResult GetProductByID([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var _product = _db.Products.FirstOrDefault(x => x.ProductID == id);
            if (_product == null)
                return NotFound();
            return Ok(_product);

        }

        [HttpPost("[action]")]
        [Authorize(Policy = "IsAdministrator")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var _product = new ProductModel()
            {
                Name = products.Name,
                Discription = products.Discription,
                OutOfStock = products.OutOfStock,
                Price = products.Price,
                ImageURL = products.ImageURL
            };
            await _db.AddAsync(_product);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "IsAdministrator")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel products)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var _product = _db.Products.FirstOrDefault(x => x.ProductID == id);
            if (_product == null)
                return NotFound();

            _product.Name = products.Name;
            _product.Discription = products.Discription;
            _product.OutOfStock = products.OutOfStock;
            _product.Price = products.Price;
            _product.ImageURL = products.ImageURL;


            _db.Entry(_product).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return Ok(new JsonResult(@"Follwing product {{id}} has been updated"));
        }

        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "IsAdministrator")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var _product = _db.Products.FirstOrDefault(x => x.ProductID == id);
            if (_product == null)
                return NotFound();
            _db.Remove(_product);
            await _db.SaveChangesAsync();
            return Ok(new JsonResult(@"Follwing product {{id}} has been deleted"));
        }
    }
}
