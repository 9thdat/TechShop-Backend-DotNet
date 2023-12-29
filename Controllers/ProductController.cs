using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShopManagementBackend.Models;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly TechShopContext _context;
        public ProductController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var productQuantities = _context.ProductQuantities.Where(pq => pq.ProductId == id).ToList();

            var productData = new
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Image = product.Image,
                Category = product.Category,
                Brand = product.Brand,
                PreDiscount = product.PreDiscount,
                DiscountPercent = product.DiscountPercent,
                Quantity = productQuantities.Sum(pq => pq.Quantity)
            };

            return Ok(productData);
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

        [HttpPut("DeleteProduct/{id}")]
        public ActionResult DeleteProduct(int id)
        {
            var productQuantities = _context.ProductQuantities.Where(pq => pq.ProductId == id);

            if (productQuantities == null)
            {
                return NotFound();
            }

            foreach (var productQuantity in productQuantities)
            {
                productQuantity.Quantity = 0;
            }
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
    }
}
