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
            var discount = _context.Discounts
                .OrderByDescending(d => d.Id)
                .ToList();
            // Lấy ngày hiện tại
            DateTime currentDate = DateTime.Now;

            // Chuyển đổi sang múi giờ GMT+7
            TimeZoneInfo gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime gmtPlus7Date = TimeZoneInfo.ConvertTime(currentDate, gmtPlus7);

            // Lấy ngày chỉ (DateOnly) từ thời gian đã chuyển đổi
            DateOnly dateOnly = DateOnly.FromDateTime(gmtPlus7Date);
            // Check if discount is expired

            foreach (var d in discount)
            {
                if (d.Status != "disabled")
                {
                    if (d.EndDate <= dateOnly)
                    {
                        d.Status = "expired";
                        d.DisabledAt = d.EndDate;
                    }
                    else
                    {
                        d.Status = "active";
                    }
                }
                else
                {
                    d.DisabledAt = d.EndDate;
                }
            }
            // Update database
            _context.SaveChanges();

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
            // Lấy ngày hiện tại
            DateTime currentDate = DateTime.Now;

            // Chuyển đổi sang múi giờ GMT+7
            TimeZoneInfo gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime gmtPlus7Date = TimeZoneInfo.ConvertTime(currentDate, gmtPlus7);

            // Lấy ngày chỉ (DateOnly) từ thời gian đã chuyển đổi
            DateOnly dateOnly = DateOnly.FromDateTime(gmtPlus7Date);
            if (discount.Status != "disabled")
            {
                if (discount.EndDate <= dateOnly)
                {
                    discount.Status = "expired";
                }
                else
                {
                    discount.Status = "active";
                }
            }
            else
            {
                discount.DisabledAt = discount.EndDate;
            }

            discount.CreatedAt = dateOnly;
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

            // Lấy ngày hiện tại
            DateTime currentDate = DateTime.Now;

            // Chuyển đổi sang múi giờ GMT+7
            TimeZoneInfo gmtPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime gmtPlus7Date = TimeZoneInfo.ConvertTime(currentDate, gmtPlus7);

            // Lấy ngày chỉ (DateOnly) từ thời gian đã chuyển đổi
            DateOnly dateOnly = DateOnly.FromDateTime(gmtPlus7Date);
            if (discountToUpdate.Status != "disabled")
            {
                if (discount.EndDate <= dateOnly)
                {
                    discountToUpdate.Status = "expired";
                    discountToUpdate.DisabledAt = discount.EndDate;
                }
                else
                {
                    discountToUpdate.Status = "active";
                }
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
            discountToUpdate.UpdatedAt = dateOnly;
            discountToUpdate.DisabledAt = discount.EndDate;

            _context.Entry(discountToUpdate).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();

        }
    }
}
