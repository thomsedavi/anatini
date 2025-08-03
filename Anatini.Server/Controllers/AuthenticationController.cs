using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Anatini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("signup")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        public IActionResult Signup([FromForm] SignupForm request)
        {
            if (request.InviteCode != "1234-5678")
            {
                return BadRequest();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Email, request.Email)
            };

            var key = Encoding.UTF8.GetBytes("ItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMe2");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "https://id.anatini.com",
                Audience = "https://api.anatini.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { Bearer = jwt });
        }
    }

    public class SignupForm
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Handle { get; set; }
        public required string InviteCode { get; set; }
    }
}
