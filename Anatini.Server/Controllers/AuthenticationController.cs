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
    public class AuthenticationController(ILogger logger) : ControllerBase
    {
        private static readonly string[] s_inviteCodeError = ["Invalid Invite Code"];
        private static readonly string[] s_passwordError = ["Incorrect Password"];

        [HttpPost("signup")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signup([FromForm] SignupForm request)
        {
            if (request.InviteCode != "1234-5678")
            {
                return BadRequest(new { Errors = new { InviteCode = s_inviteCodeError } });
            }

            try
            {
                using var context = new AnatiniContext();
                
                var id = Guid.NewGuid();

                context.Users.Add(new User
                {
                    Id = id,
                    Email = request.Email,
                    Name = request.Name,
                    Password = request.Password
                });

                await context.SaveChangesAsync();
         
                return Ok(new { Bearer = GetBearer(id) });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception creating user");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult Login([FromForm] LoginForm request)
        {
            if (request.Password != "password")
            {
                return BadRequest(new { Errors = new { Password = s_passwordError } });
            }

            return Ok(new { Bearer = GetBearer(Guid.NewGuid()) });
        }

        private static string GetBearer(Guid id)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, id.ToString())
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

            return tokenHandler.WriteToken(token);
        }
    }

    // [JsonPropertyName] should format to camelCase but for some reason not currently working, figure out why later
    // https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-9.0#configure-formatters-2
    public class SignupForm
    {
        [StringLength(60), DataType(DataType.Text)]
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

    public class LoginForm
    {
        [DataType(DataType.EmailAddress)]
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
