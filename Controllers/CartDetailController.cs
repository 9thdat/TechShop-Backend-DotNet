using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShopManagementBackend.Models;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartDetailController : ControllerBase
    {
        private readonly TechShopContext _context;

        public CartDetailController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetCartDetail()
        {
            var cartDetail = _context.CartDetails;
            return Ok(cartDetail);
        }

        [HttpGet("{id}")]
        public ActionResult GetCartDetail(int id)
        {
            var cartDetail = _context.CartDetails.Find(id);
            if (cartDetail == null)
            {
                return NotFound();
            }
            return Ok(cartDetail);
        }

        [HttpPost]
        public ActionResult CreateCartDetail(CartDetail cartDetail)
        {
            _context.CartDetails.Add(cartDetail);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCartDetail), new { id = cartDetail.Id }, cartDetail);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCartDetail(int id, CartDetail cartDetail)
        {
            if (id != cartDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(cartDetail).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCartDetail(int id)
        {
            var cartDetail = _context.CartDetails.Find(id);
            if (cartDetail == null)
            {
                return NotFound();
            }

            _context.CartDetails.Remove(cartDetail);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
