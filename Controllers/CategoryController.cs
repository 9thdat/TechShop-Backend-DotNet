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
    public class CategoryController : ControllerBase
    {
        private readonly TechShopContext _context;

        public CategoryController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetCategory()
        {
            var category = _context.Categories;
            return Ok(category);
        }
    }
}
