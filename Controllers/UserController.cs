using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhoneShopManagementBackend.Models;
using PhoneShopManagementBackend.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PhoneShopManagementBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly TechShopContext _context;
        private readonly AppSettings _appSettings;

        public UserController(TechShopContext context, AppSettings appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        [HttpGet("Staffs")]
        public ActionResult GetStaffs()
        {
            var staffs = _context.Users.Where(u => u.Role == "staff");
            staffs.ToList().ForEach(s => s.Password = "");
            return Ok(staffs);
        }

        [HttpGet("Staffs/Valid/email={email}")]
        public ActionResult GetStaff(string email)
        {
            var staff = _context.Users.Find(email);
            if (staff != null)
            {
                return Conflict();
            }
            return Ok();
        }

        [HttpGet("{email}")]
        public ActionResult GetUser(string email)
        {
            var user = _context.Users.Find(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public IActionResult ValidateUser([FromBody] UserCredentials credentials)
        {
            if (credentials == null)
            {
                return BadRequest(new { Message = "Invalid client request" });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == credentials.Email);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            if (PasswordHasher.VerifyPassword(credentials.Password, user.Password) == false)
            {
                return Unauthorized(new { Message = "Invalid password" });
            }

            var role = user.Role;
            var status = user.Status;
            var token = GenerateToken(user);

            return Ok(new
            {
                role = role,
                status = status,
                token = token
            });
        }

        public class UserCredentials
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("ValidateToken")]
        [AllowAnonymous]
        public IActionResult ValidateToken([FromBody] TokenRequest request)
        {
            var token = request.Token;
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = Encoding.ASCII.GetBytes(_appSettings.SecretKey);

            try
            {
                jwtTokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Extract claims from the validated token
                var claims = ((JwtSecurityToken)validatedToken).Claims;

                // Return the claims
                return Ok(claims.Select(c => new { Type = c.Type, Value = c.Value }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return Unauthorized();
            }
        }


        public class TokenRequest
        {
            public string Token { get; set; }
        }

        [HttpPost("Staff")]
        public ActionResult CreateStaff(User user)
        {
            user.Password = PasswordHasher.HashPassword(user.Password);

            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUser), new { email = user.Email }, user);
        }

        [HttpPut("Staffs/Update")]
        public ActionResult UpdateStaff(User user)
        {
            var staff = _context.Users.Find(user.Email);
            if (staff == null)
            {
                return NotFound();
            }

            staff.Name = user.Name;
            staff.Phone = user.Phone;
            staff.Address = user.Address;
            staff.Gender = user.Gender;
            staff.Birthday = user.Birthday;
            staff.Status = user.Status;
            staff.City = user.City;
            staff.District = user.District;
            staff.Ward = user.Ward;
            staff.Image = user.Image;

            _context.Entry(staff).State = EntityState.Modified;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPut("ChangePassword/{email}")]
        public ActionResult ChangePassword(string email)
        {
            var userInDb = _context.Users.Find(email);
            if (userInDb == null)
            {
                return NotFound();
            }

            userInDb.Password = PasswordHasher.HashPassword(userInDb.Password);
            _context.Entry(userInDb).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        private string GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var secretKeyBytes = System.Text.Encoding.ASCII.GetBytes(_appSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Email),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
                }),
                Expires = System.DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }
    }
}
