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
    public class ParameterAdapterController : ControllerBase
    {
        private readonly TechShopContext _context;

        public ParameterAdapterController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetParameterAdapter()
        {
            var parameterAdapter = _context.ParameterAdapters;
            return Ok(parameterAdapter);
        }

        [HttpGet("{id}")]
        public ActionResult GetParameterAdapter(int id)
        {
            var parameterAdapter = _context.ParameterAdapters.Find(id);
            if (parameterAdapter == null)
            {
                return NotFound();
            }
            return Ok(parameterAdapter);
        }

        [HttpGet("ProductId={id}")]
        public ActionResult GetParameterAdapterByProductId(int id)
        {
            var parameterAdapter = _context.ParameterAdapters.Where(p => p.ProductId == id);
            if (parameterAdapter == null)
            {
                return Ok();
            }

            return Ok(parameterAdapter);
        }

        [HttpPost]
        public ActionResult CreateParameterAdapter(ParameterAdapter parameterAdapter)
        {
            _context.ParameterAdapters.Add(parameterAdapter);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateParameterAdapter(int id, ParameterAdapter parameterAdapter)
        {
            if (id != parameterAdapter.Id)
            {
                return BadRequest();
            }

            _context.Entry(parameterAdapter).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteParameterAdapter(int id)
        {
            var parameterAdapter = _context.ParameterAdapters.Find(id);
            if (parameterAdapter == null)
            {
                return NotFound();
            }

            _context.ParameterAdapters.Remove(parameterAdapter);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
