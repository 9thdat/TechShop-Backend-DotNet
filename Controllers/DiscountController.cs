using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShopManagementBackend.Models;
using System.Runtime.InteropServices.JavaScript;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DiscountController : ControllerBase
    {
        private readonly TechShopContext _context;

        public DiscountController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetDiscount()
        {
            var discount = _context.Discounts;
            return Ok(discount);
        }

        [HttpGet("{id}")]
        public ActionResult GetDiscount(int id)
        {
            var discount = _context.Discounts.Find(id);
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }


        [HttpGet("Code={code}")]
        public ActionResult GetDiscount(string code)
        {
            var discount = _context.Discounts.Where(d => d.Code == code).FirstOrDefault();
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount);
        }

        [HttpGet("GetLastId")]
        public ActionResult GetLastId()
        {
            var discount = _context.Discounts.OrderByDescending(d => d.Id).FirstOrDefault();
            if (discount == null)
            {
                return NotFound();
            }
            return Ok(discount.Id);
        }

        [HttpPost]
        public ActionResult CreateDiscount(Discount discount)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Today);

            if (discount.EndDate < currentDate)
            {
                discount.Status = "expired";
            }
            else
            {
                discount.Status = "active";
            }

            discount.CreatedAt = currentDate;
            _context.Discounts.Add(discount);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetDiscount), new { id = discount.Id }, discount);
        }

        [HttpPut]
        public ActionResult UpdateDiscount(Discount discount)
        {
            var discountToUpdate = _context.Discounts.Find(discount.Id);
            if (discountToUpdate == null)
            {
                return NotFound();
            }

            if (discount.EndDate < DateOnly.FromDateTime(DateTime.Today))
            {
                discountToUpdate.Status = "expired";
            }
            else
            {
                discountToUpdate.Status = "active";
            }

            discountToUpdate.Code = discount.Code;
            discountToUpdate.Type = discount.Type;
            discountToUpdate.Value = discount.Value;
            discountToUpdate.Description = discount.Description;
            discountToUpdate.StartDate = discount.StartDate;
            discountToUpdate.EndDate = discount.EndDate;
            discountToUpdate.MinApply = discount.MinApply;
            discountToUpdate.MaxSpeed = discount.MaxSpeed;
            discountToUpdate.Quantity = discount.Quantity;
            discountToUpdate.UpdatedAt = DateOnly.FromDateTime(DateTime.Today);
            discountToUpdate.DisabledAt = discount.EndDate;

            _context.Entry(discountToUpdate).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();

        }
    }
}
