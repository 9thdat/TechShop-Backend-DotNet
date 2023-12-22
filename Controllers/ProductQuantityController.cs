using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhoneShopManagementBackend.Models;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductQuantityController : ControllerBase
    {
        private readonly TechShopContext _context;

        public ProductQuantityController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.ProductQuantities);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var productQuantity = _context.ProductQuantities.Find(id);
            if (productQuantity == null)
            {
                return NotFound();
            }

            return Ok(productQuantity);
        }

        [HttpGet("ProductId={id}")]
        public IActionResult GetByProductId(int id)
        {
            var products = _context.ProductQuantities.Where(p => p.ProductId == id);

            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpGet("TotalQuantity/ProductId={id}&ProductColor={color}")]
        public IActionResult GetTotalQuantity(int id, string color)
        {
            var totalQuantity = _context.ProductQuantities.Where(p => p.ProductId == id && p.Color == color).Sum(p => p.Quantity);
            if (totalQuantity == 0)
            {
                return NotFound();
            }
            return Ok(totalQuantity);
        }

        [HttpGet("GetLastId")]
        public IActionResult GetLastId()
        {
            var productQuantity = _context.ProductQuantities.Find(_context.ProductQuantities.Count());
            if (productQuantity == null)
            {
                return Ok();
            }

            return Ok(productQuantity);
        }

        [HttpPost]
        public IActionResult Post(ProductQuantity productQuantity)
        {
            _context.ProductQuantities.Add(productQuantity);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, ProductQuantity productQuantity)
        {
            var productQuantityInDb = _context.ProductQuantities.Find(id);
            if (productQuantityInDb == null)
            {
                return NotFound();
            }

            productQuantityInDb.Color = productQuantity.Color;
            productQuantityInDb.Quantity = productQuantity.Quantity;
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var productQuantityInDb = _context.ProductQuantities.Find(id);
            if (productQuantityInDb == null)
            {
                return NotFound();
            }

            _context.ProductQuantities.Remove(productQuantityInDb);
            _context.SaveChanges();
            return Ok();
        }

    }
}
