using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShopManagementBackend.Models;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly TechShopContext _context;
        public ProductController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetProduct()
        {
            var product = _context.Products;
            return Ok(product);
        }

        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("GetProductAndQuantity")]
        public ActionResult GetProductAndQuantity()
        {
            var products = _context.Products
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Description,
                    p.Image,
                    p.Category,
                    p.Brand,
                    p.PreDiscount,
                    p.DiscountPercent,
                    Quantity = _context.ProductQuantities
                        .Where(pq => pq.ProductId == p.Id)
                        .Sum(pq => pq.Quantity)
                });

            return Ok(products);
        }

        [HttpGet("GetLastId")]
        public ActionResult GetLastId()
        {
            var lastId = _context.Products.OrderByDescending(p => p.Id).FirstOrDefault();
            if (lastId == null)
            {
                return NotFound();
            }
            return Ok(lastId.Id);
        }

        [HttpPost]
        public ActionResult CreateProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok();
        }

    }
}
