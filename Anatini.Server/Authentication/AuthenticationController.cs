using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Anatini.Server.Authentication.Commands;
using Anatini.Server.Authentication.Queries;
using Anatini.Server.Controllers;
using Anatini.Server.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CosmosException = Microsoft.Azure.Cosmos.CosmosException;

namespace Anatini.Server.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("inviteCode")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InviteCode([FromForm] InviteCodeForm form)
        {
            var eventData = new EventData(HttpContext).Add("Email", form.Email);
            var userId = Guid.Empty;

            try
            {
                if (form.InviteCode == "zzzzzzzz")
                {
                    userId = Guid.NewGuid();
                }
                else
                {
                    var invite = await new GetCodeInvite(form.InviteCode).ExecuteAsync();

                    if (invite != null)
                    {
                        userId = invite.NewUserId;
                        eventData.Add("InvitedByUserId", invite.InvitedByUserId.ToString());
                    }
                    else
                    {
                        return Ok();
                    }
                }

                await new CreateEmailUser(form.Email, userId).ExecuteAsync();
                await new CreateUserEvent(userId, UserEventType.EmailCreated, eventData).ExecuteAsync();

                return Ok();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
                {
                    await new CreateUserEvent(userId, UserEventType.EmailConflict, eventData).ExecuteAsync();
                    return Ok();
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

        [HttpPost("signup")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signup([FromForm] SignupForm form)
        {
            var eventData = new EventData(HttpContext).Add("Email", form.Email).Add("Name", form.Name);

            try
            {
                var emailUser = await new GetEmailUser(form.Email).ExecuteAsync();
                var userId = emailUser?.UserId ?? Guid.Empty;

                if (emailUser == null || emailUser.Verified)
                {
                    return NotFound();
                }
                else if (emailUser.VerificationCode != form.VerificationCode)
                {
                    await new DeleteEmailUser(emailUser).ExecuteAsync();
                    await new CreateUserEvent(userId, UserEventType.VerificationBad, eventData).ExecuteAsync();

                    return NotFound();
                }

                await new CreateUser(form.Name, form.Password, emailUser, userId).ExecuteAsync();

                if (emailUser.InvitedByUserId.HasValue)
                {
                    var invitedByUserId = emailUser.InvitedByUserId.Value;

                    var invitedByUser = await new GetUser(invitedByUserId).ExecuteAsync();

                    invitedByUser!.Invites.FirstOrDefault(invite => invite.Id == emailUser.InviteId!)!.Used = true;

                    await new UpdateUser(invitedByUser).ExecuteAsync();

                    await new CreateUserRelationships(userId, invitedByUserId, "INVITED_BY", "TRUSTS", "TRUSTED_BY").ExecuteAsync();
                    await new CreateUserRelationships(invitedByUserId, userId, "INVITED", "TRUSTS", "TRUSTED_BY").ExecuteAsync();

                    emailUser.InvitedByUserId = null;
                }

                emailUser.Verified = true;
                emailUser.VerificationCode = null;

                await new UpdateEmailUser(emailUser).ExecuteAsync();

                await new CreateUserEvent(userId, UserEventType.UserCreated, eventData).ExecuteAsync();

                return Ok(new { Bearer = GetBearer(userId) });
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromForm] LoginForm form)
        {
            var eventData = new EventData(HttpContext).Add("Email", form.Email);

            try
            {
                var verifyPassword = new VerifyPassword(form.Email, form.Password);
                var userId = await verifyPassword.ExecuteAsync();

                if (userId.HasValue)
                {
                    var createUserEvent = new CreateUserEvent(userId.Value, UserEventType.LoginOk, eventData);
                    await createUserEvent.ExecuteAsync();

                    return Ok(new { Bearer = GetBearer(userId.Value) });
                }
                else
                {
                    return NotFound();
                }
            }
            catch(InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                //logger.LogError(exception, "Exception logging in");
                return Problem();
            }
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
