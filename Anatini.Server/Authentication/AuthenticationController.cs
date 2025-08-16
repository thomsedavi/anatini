using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Anatini.Server.Authentication.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CosmosException = Microsoft.Azure.Cosmos.CosmosException;

namespace Anatini.Server.Authentication
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signup([FromForm] SignupForm request)
        {
            try
            {
                if (request.InviteCode != "12345678")
                {
                    return BadRequest(new { Errors = new { InviteCode = s_inviteCodeError } });
                }

                var userId = Guid.NewGuid();
                var createdDate = DateOnly.FromDateTime(DateTime.Now);

                var createEmailUser = new CreateEmailUser(request.Email, userId, createdDate);
                await createEmailUser.ExecuteAsync();

                var createUser = new CreateUser(request.Name, request.Password, request.Email, userId, createdDate);
                await createUser.ExecuteAsync();

                return Ok(new { Bearer = GetBearer(userId) });
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
                    return Problem();
                }
            }
            catch (Exception)
            {
                //logger.LogError(exception, "Exception creating user");
                return Problem();
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

        [Authorize]
        [HttpPost("verifyEmail")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult VerifyEmail([FromForm] VerifyEmailForm request)
        {
            return Ok(new { Bearer = GetBearer(Guid.NewGuid()) });
        }

        private static string GetBearer(Guid id)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, id.ToString()),
                new(ClaimTypes.Role, "Verified?")
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
}
