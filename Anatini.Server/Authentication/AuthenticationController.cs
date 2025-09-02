using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Anatini.Server.Commands;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Queries;
using Anatini.Server.Utils;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InviteCode()
        {
            var eventData = new EventData(HttpContext);

            try
            {
                var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    var user = await new GetUser(userId).ExecuteAsync();

                    if (user.Invites?.Any(invite => invite.CreatedDateNZ == eventData.DateOnlyNZNow) ?? false)
                    {
                        return Conflict();
                    }
                    else
                    {
                        var inviteId = Guid.NewGuid();
                        var inviteCode = CodeRandom.Next();
                        var attemptCount = 0;
                        var success = false;

                        while (attemptCount++ < 5 && !success)
                        {
                            try
                            {
                                await new CreateCodeInvite(inviteCode, userId, inviteId, eventData.DateOnlyNZNow).ExecuteAsync();
                                success = true;
                            }
                            catch (Exception)
                            {
                                inviteCode = CodeRandom.Next();
                            }
                        }

                        if (!success)
                        {
                            return Problem();
                        }

                        var userInvite = new UserInvite
                        {
                            Id = inviteId,
                            InviteCode = inviteCode,
                            Used = false,
                            CreatedDateNZ = eventData.DateOnlyNZNow
                        };

                        var invites = (user.Invites ?? []).ToList();
                        invites.Add(userInvite);
                        user.Invites = invites;

                        await new UpdateUser(user).ExecuteAsync();
                        await new CreateUserEvent(userId, UserEventType.InviteCreated, eventData).ExecuteAsync();

                        return Created("?", new UserDto(user));
                    }
                }
                else
                {
                    return Problem();
                }
            }
            catch (Exception)
            {
                return Problem();
            }
        }


        [HttpPost("email")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Email([FromForm] EmailForm form)
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

                var refreshToken = TokenGenerator.Get;

                await new CreateUser(form.Name, form.Password, emailUser, userId, refreshToken, eventData).ExecuteAsync();

                if (emailUser.InvitedByUserId.HasValue)
                {
                    var invitedByUserId = emailUser.InvitedByUserId.Value;

                    var invitedByUser = await new GetUser(invitedByUserId).ExecuteAsync();

                    invitedByUser.Invites!.First(invite => invite.Id == emailUser.InviteId).Used = true;

                    await new UpdateUser(invitedByUser).ExecuteAsync();

                    await new CreateUserRelationships(userId, invitedByUserId, "INVITED_BY", "TRUSTS", "TRUSTED_BY").ExecuteAsync();
                    await new CreateUserRelationships(invitedByUserId, userId, "INVITED", "TRUSTS", "TRUSTED_BY").ExecuteAsync();

                    emailUser.InvitedByUserId = null;
                }

                emailUser.Verified = true;
                emailUser.VerificationCode = null;

                await new UpdateEmailUser(emailUser).ExecuteAsync();

                await new CreateUserEvent(userId, UserEventType.UserCreated, eventData).ExecuteAsync();

                var accessToken = GetAccessToken(userId, eventData.DateTimeUtc);

                AppendCookies(accessToken, refreshToken, eventData.DateTimeUtc);

                return Ok();
            }
            catch (Exception)
            {
                //logger.LogError(exception, "Exception creating user");
                return Problem();
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return Ok();
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
                var user = await new VerifyPassword(form.Email, form.Password).ExecuteAsync();

                if (user != null)
                {
                    await new CreateUserEvent(user.Id, UserEventType.LoginOk, eventData).ExecuteAsync();

                    var refreshToken = TokenGenerator.Get;

                    var userRefreshToken = new UserRefreshToken
                    {
                        Id = Guid.NewGuid(),
                        RefreshToken = refreshToken,
                        CreatedDateNZ = eventData.DateOnlyNZNow,
                        IPAddress = eventData.Get("IPAddress"),
                        UserAgent = eventData.Get("UserAgent"),
                        Revoked = false
                    };

                    var refreshTokens = user.RefreshTokens.ToList();
                    refreshTokens.Add(userRefreshToken);
                    user.RefreshTokens = refreshTokens;

                    await new UpdateUser(user).ExecuteAsync();

                    var accessToken = GetAccessToken(user.Id, eventData.DateTimeUtc);

                    AppendCookies(accessToken, refreshToken, eventData.DateTimeUtc);

                    return Ok();
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
            return Problem();
        }

        [HttpGet("isAuthenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult IsAuthenticated()
        {
            return Ok(new { User.Identity?.IsAuthenticated });
        }

        private static string GetAccessToken(Guid id, DateTime dateTimeUTC)
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
                Expires = dateTimeUTC.AddHours(1),
                Issuer = "https://id.anatini.com",
                Audience = "https://api.anatini.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private void AppendCookies(string accessToken, string refreshToken, DateTime dateTimeUtc)
        {
            var accessTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = dateTimeUtc.AddHours(1)
            };

            var refreshTokenCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = dateTimeUtc.AddDays(28)
            };

            Response.Cookies.Append("access_token", accessToken, accessTokenCookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken, refreshTokenCookieOptions);
        }
    }
}
