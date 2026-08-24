using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config) => _config = config;

        [HttpPost("login")]
        public IActionResult Login()
        {
            // 1. Define the security key (must match what you have in Program.cs!)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key-at-least-16-chars"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2. Create the claims (User info inside the token)
            var claims = new[] { new Claim(ClaimTypes.Name, "TestUser") };

            // 3. Generate the token
            var token = new JwtSecurityToken(
                issuer: "your-issuer",
                audience: "your-audience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
