using Microsoft.AspNetCore.Mvc;
using UsersWebApi_Module3.Data;
using UsersWebApi_Module3.Models;
using System.Linq;

namespace UsersWebApi_Module3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        
        // 🔹 Register endpoint
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDto userDto)
        {
            if (_context.Users.Any(u => u.Username == userDto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = userDto.Username,
                Password = userDto.Password   // ❌ plain text, but simple for now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully");
        }

        // 🔹 Login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto userDto)
        {
            var user = _context.Users
                .SingleOrDefault(u => u.Username == userDto.Username && u.Password == userDto.Password);

            if (user == null)
                return Unauthorized("Invalid username or password");

            return Ok("Login successful");
        }
    }
}

