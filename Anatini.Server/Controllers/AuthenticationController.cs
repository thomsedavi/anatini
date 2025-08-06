using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Anatini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private static readonly string[] s_inviteCodeError = ["Invalid Invite Code"];

        [HttpPost("signup")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult Signup([FromForm] SignupForm request)
        {
            if (request.InviteCode != "1234-5678")
            {
                return BadRequest(new { Errors = new { InviteCode = s_inviteCodeError } } );
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

    // [JsonPropertyName] should format to camelCase but for some reason not currently working
    // https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-9.0#configure-formatters-2
    public class SignupForm
    {
        [StringLength(60, MinimumLength = 3), DataType(DataType.Text)]
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [DataType(DataType.EmailAddress)]
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public required string Password { get; set; }
        [Display(Name = "Invite Code"), DataType(DataType.Text)]
        [JsonPropertyName("inviteCode")]
        public required string InviteCode { get; set; }
    }
}
