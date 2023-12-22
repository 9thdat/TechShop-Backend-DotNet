using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneShopManagementBackend.Models;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly TechShopContext _context;

        public CustomerController(TechShopContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetCustomer()
        {
            var customers = _context.Customers
                .Select(c => new
                {
                    Email = c.Email,
                    Name = c.Name,
                    Phone = c.Phone,
                    Gender = c.Gender,
                    Birthday = c.Birthday,
                    Address = c.Address,
                    Ward = c.Ward,
                    District = c.District,
                    City = c.City,
                    Status = c.Status,
                    Image = c.Image,
                    // Set the password to an empty string
                    Password = ""
                })
                .ToList();

            return Ok(customers);
        }

        [HttpPost]
        public ActionResult CreateCustomer(Customer customer)
        {
            var existingCustomer = _context.Customers.Find(customer.Email);

            if (existingCustomer == null)
            {
                // Email is unique, proceed to create the customer
                // Hash the password using SHA-256 (replace "HashPassword" with your actual hashing method)
                customer.Password = PasswordHasher.HashPassword(customer.Password);

                _context.Customers.Add(customer);
                _context.SaveChanges();

                // Return a JSON response with status 201 (Created)
                return CreatedAtAction(nameof(GetCustomer), new { email = customer.Email }, new { status = "201" });
            }
            else
            {
                // Email already exists, return a response indicating the conflict
                return Conflict(new
                {
                    status = "409",
                    message = "Email already exists"
                });
            }
        }


        [HttpGet("{email}")]
        public ActionResult GetCustomer(string email)
        {
            var customer = _context.Customers.Find(email);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPut]
        public ActionResult UpdateCustomer([FromForm] string email, [FromForm] Customer customer)
        {
            if (email != customer.Email)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("ChangeStatus/Email={Email}")]
        public ActionResult ChangeCurrentStatus(string Email)
        {
            var customer = _context.Customers.Find(Email);
            if (customer == null)
            {
                return NotFound();
            }

            customer.Status = customer.Status == "active" ? "inactive" : "active";
            _context.Entry(customer).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{email}")]
        public ActionResult DeleteCustomer(string email)
        {
            var customer = _context.Customers.Find(email);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
