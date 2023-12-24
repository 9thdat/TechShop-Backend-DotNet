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
    public class ParameterBackupchargerController : ControllerBase
    {
        private readonly TechShopContext _context;

        public ParameterBackupchargerController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetParameterBackupcharger()
        {
            var parameterBackupcharger = _context.ParameterBackupchargers;
            return Ok(parameterBackupcharger);
        }

        [HttpGet("ProductId={productId}")]
        public ActionResult GetParameterBackupchargerByProductId(int productId)
        {
            var parameterBackupcharger = _context.ParameterBackupchargers.Where(p => p.ProductId == productId);
            if (parameterBackupcharger == null)
            {
                return Ok();
            }

            return Ok(parameterBackupcharger);
        }

        [HttpPost]
        public ActionResult CreateParameterBackupcharger(ParameterBackupcharger parameterBackupcharger)
        {
            _context.ParameterBackupchargers.Add(parameterBackupcharger);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateParameterBackupcharger(int id, ParameterBackupcharger parameterBackupcharger)
        {
            if (id != parameterBackupcharger.Id)
            {
                return BadRequest();
            }

            _context.Entry(parameterBackupcharger).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteParameterBackupcharger(int id)
        {
            var parameterBackupcharger = _context.ParameterBackupchargers.Find(id);
            if (parameterBackupcharger == null)
            {
                return NotFound();
            }

            _context.ParameterBackupchargers.Remove(parameterBackupcharger);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
