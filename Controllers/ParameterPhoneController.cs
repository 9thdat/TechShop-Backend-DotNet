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
    public class ParameterPhoneController : ControllerBase
    {
        private readonly TechShopContext _context;

        public ParameterPhoneController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetParameterPhone()
        {
            var parameterPhone = _context.ParameterPhones;
            return Ok(parameterPhone);
        }

        [HttpGet("ProductId={id}")]
        public ActionResult GetParameterPhoneByProductId(int id)
        {
            var parameterPhone = _context.ParameterPhones.Where(p => p.ProductId == id);
            if (parameterPhone == null)
            {
                return Ok();
            }

            return Ok(parameterPhone);
        }

        [HttpPost]
        public ActionResult CreateParameterPhone(ParameterPhone parameterPhone)
        {
            _context.ParameterPhones.Add(parameterPhone);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateParameterPhone(int id, ParameterPhone parameterPhone)
        {
            if (id != parameterPhone.Id)
            {
                return BadRequest();
            }

            _context.Entry(parameterPhone).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteParameterPhone(int id)
        {
            var parameterPhone = _context.ParameterPhones.Find(id);
            if (parameterPhone == null)
            {
                return NotFound();
            }

            _context.ParameterPhones.Remove(parameterPhone);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
