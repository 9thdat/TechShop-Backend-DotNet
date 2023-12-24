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
    public class ImageDetailController : ControllerBase
    {
        private readonly TechShopContext _context;

        public ImageDetailController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetImageDetail()
        {
            var imageDetail = _context.ImageDetails;
            return Ok(imageDetail);
        }

        [HttpGet("ProductId={id}")]
        public ActionResult GetImageByProductId(int id)
        {
            var imageDetail = _context.ImageDetails.Where(p => p.ProductId == id);
            if (imageDetail == null)
            {
                return NotFound();
            }

            return Ok(imageDetail);
        }

        [HttpPost]
        public ActionResult CreateImageDetail(ImageDetail imageDetail)
        {
            _context.ImageDetails.Add(imageDetail);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateImageDetail(int id, ImageDetail imageDetail)
        {
            if (id != imageDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(imageDetail).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteImageDetail(int id)
        {
            var imageDetail = _context.ImageDetails.Find(id);
            if (imageDetail == null)
            {
                return NotFound();
            }

            _context.ImageDetails.Remove(imageDetail);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
