using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CosmosException = Microsoft.Azure.Cosmos.CosmosException;

namespace Anatini.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController() : ControllerBase
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
            if (request.InviteCode != "12345678")
            {
                return BadRequest(new { Errors = new { InviteCode = s_inviteCodeError } });
            }
            
            using var context = new AnatiniContext();

            var userId = Guid.NewGuid();
            var createdDate = DateOnly.FromDateTime(DateTime.Now);

            try
            {
                context.EmailUsers.Add(new EmailUser
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    UserId = userId,
                    CreatedDate = createdDate
                });

                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    return Conflict();
                }
                else
                {
                    //logger.LogError(dbUpdateException, "Exception creating user");
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            catch (Exception exception)
            {
                //logger.LogError(exception, "Exception creating user");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            try
            {
                context.Users.Add(new User
                {
                    Id = userId,
                    Name = request.Name,
                    Password = request.Password,
                    CreatedDate = createdDate
                });

                context.UserEmails.Add(new UserEmail
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Email = request.Email,
                    CreatedDate = createdDate
                });

                await context.SaveChangesAsync();
         
                return Ok(new { Bearer = GetBearer(userId) });
            }
            catch (Exception exception)
            {
                //logger.LogError(exception, "Exception creating user");
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
