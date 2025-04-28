using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementAPI.DataBase;
using TaskManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementAPI.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginModel)
        {
            // Simple validation
            if (string.IsNullOrEmpty(loginModel.Username))
                return BadRequest("Username is required");
            if (string.IsNullOrEmpty(loginModel.Password))
                return BadRequest("Password is required");

            // Case-insensitive search
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == loginModel.Username.Trim().ToLower());

            if (user == null) return Unauthorized("Invalid credentials");

            // Exact password match (trimmed)
            if (user.Password != loginModel.Password.Trim())
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                Token = GenerateJwtToken(user),
                User = new { user.Id, user.Username, user.Role }
            });
        }
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),  
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
