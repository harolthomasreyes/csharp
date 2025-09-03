using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using testing.Entities;

namespace testing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        public AuthController(IConfiguration configuration, IJwtService jwtService) 
        { 
            this._configuration = configuration;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin user)
        {
            if (user.Username != "admin" || user.Password != "password")
            {
                return Unauthorized("invalid credentials");
            }

            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "Admin"),
        };

            var token = _jwtService.GenerateToken(claims);

            return Ok(new { Token = token });
        }
    }
}
