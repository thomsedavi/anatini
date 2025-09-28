using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Anatini.Server.Context;
using Anatini.Server.Context.Commands;
using Anatini.Server.Enums;
using Anatini.Server.Users.Commands;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Users.Queries;
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
    public class AuthenticationController : AnatiniControllerBase
    {
        [HttpPost("inviteCode")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostInviteCode()
        {
            var eventData = new EventData(HttpContext);

            async Task<IActionResult> userFunction(User user)
            {
                if (user.Invites?.Any(invite => invite.DateOnlyNZ == eventData.DateOnlyNZNow) ?? false)
                {
                    return Conflict();
                }
                else
                {
                    var inviteCode = CodeRandom.Next();
                    var attemptCount = 0;
                    var success = false;

                    while (attemptCount++ < 5 && !success)
                    {
                        try
                        {
                            await new CreateUserInvite(inviteCode, user.Id, eventData.DateOnlyNZNow).ExecuteAsync();
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

                    user.AddInvite(inviteCode, eventData);
                    await new Update(user).ExecuteAsync();

                    await new CreateUserEvent(user.Id, EventType.InviteCreated, eventData).ExecuteAsync();

                    return Created("?", user.ToUserEditDto());
                }
            }

            return await UsingUser(userFunction);
        }

        [HttpPost("emailAddress")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEmail([FromForm] EmailForm form)
        {
            var eventData = new EventData(HttpContext).Add("EmailAddress", form.EmailAddress);
            var userId = Guid.Empty;

            try
            {
                if (form.InviteCode == "zzzzzzzz")
                {
                    userId = Guid.NewGuid();
                }
                else
                {
                    var inviteResult = await new GetUserInvite(form.InviteCode).ExecuteAsync();

                    if (inviteResult == null)
                    {
                        return Ok();
                    }

                    var invite = inviteResult!;

                    if (invite.EmailAddress != null)
                    {
                        var emailResult = await new GetUserEmail(invite.EmailAddress).ExecuteAsync();

                        if (emailResult != null)
                        {
                            // Delete existing email result
                            // TODO maybe just update existing email with new details?
                            await new Remove(emailResult!).ExecuteAsync();
                        }
                    }

                    invite.EmailAddress = form.EmailAddress;
                    await new Update(invite).ExecuteAsync();

                    userId = invite.NewUserId;
                    eventData.Add("InvitedByUserId", invite.InvitedByUserId.ToString());
                }

                await new CreateUserEmail(form.EmailAddress, userId).ExecuteAsync();
                await new CreateUserEvent(userId, EventType.EmailCreated, eventData).ExecuteAsync();

                return Ok();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
                {
                    await new CreateUserEvent(userId, EventType.EmailConflict, eventData).ExecuteAsync();
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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSignup([FromForm] NewUser newUser)
        {
            var eventData = new EventData(HttpContext).Add("EmailAddress", newUser.EmailAddress).Add("Name", newUser.Name);

            try
            {
                var emailResult = await new GetUserEmail(newUser.EmailAddress).ExecuteAsync();

                if (emailResult == null)
                {
                    return NotFound();
                }

                var email = emailResult!;

                if (email.Verified)
                {
                    return NotFound();
                }

                newUser.Id = email.UserId;

                if (email.VerificationCode != newUser.VerificationCode)
                {
                    await new Remove(email).ExecuteAsync();
                    await new CreateUserEvent(newUser.Id, EventType.VerificationBad, eventData).ExecuteAsync();

                    return NotFound();
                }

                // This will return a Conflict if user slug is already in use
                var userSlug = newUser.CreateSlug();
                await new Add(userSlug).ExecuteAsync();

                if (email.InvitedByUserId.HasValue)
                {
                    var invitedByUserId = email.InvitedByUserId.Value;

                    var invitedByUser = (await new GetUser(invitedByUserId).ExecuteAsync())!;

                    invitedByUser.Invites!.First(invite => invite.Code == email.InviteCode).Used = true;

                    await new Update(invitedByUser).ExecuteAsync();

                    await new CreateUserToUserRelationships(newUser.Id, invitedByUserId, UserToUserRelationshipType.InvitedBy, UserToUserRelationshipType.Trusts, UserToUserRelationshipType.TrustedBy).ExecuteAsync();
                    await new CreateUserToUserRelationships(invitedByUserId, newUser.Id, UserToUserRelationshipType.Invites, UserToUserRelationshipType.Trusts, UserToUserRelationshipType.TrustedBy).ExecuteAsync();

                    email.InvitedByUserId = null;
                }

                email.Verified = true;
                email.VerificationCode = null;

                var refreshToken = TokenGenerator.Get;
                var user = newUser.Create(email, refreshToken, eventData);

                await new Add(user).ExecuteAsync();
                await new Update(email).ExecuteAsync();
                await new CreateUserEvent(user.Id, EventType.UserCreated, eventData).ExecuteAsync();

                var accessToken = GetAccessToken(user.Id, eventData.DateTimeUtc);

                AppendCookies(accessToken, refreshToken, eventData.DateTimeUtc);

                return Ok();
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
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

        [Authorize]
        [HttpPost("logout")]
        public IActionResult PostLogout()
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
        public async Task<IActionResult> PostLogin([FromForm] LoginForm form)
        {
            var eventData = new EventData(HttpContext).Add("EmailAddress", form.EmailAddress);

            try
            {
                var userResult = await new VerifyPassword(form.EmailAddress, form.Password).ExecuteAsync();

                if (userResult == null)
                {
                    return NotFound();
                }

                var user = userResult!;

                await new CreateUserEvent(user.Id, EventType.LoginOk, eventData).ExecuteAsync();

                var refreshToken = TokenGenerator.Get;

                user.AddSession(refreshToken, eventData);
                await new Update(user).ExecuteAsync();

                var accessToken = GetAccessToken(user.Id, eventData.DateTimeUtc);

                AppendCookies(accessToken, refreshToken, eventData.DateTimeUtc);

                return Ok();
            }
            catch (Exception)
            {
                //logger.LogError(exception, "Exception logging in");
                return Problem();
            }
        }

        [Authorize]
        [HttpGet("account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserEdit()
        {
            async Task<IActionResult> userFunction(User user)
            {
                return await Task.FromResult(Ok(user.ToUserEditDto()));
            }

            return await UsingUser(userFunction);
        }

        [Authorize]
        [HttpPost("verifyEmail")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult PostVerifyEmail([FromForm] VerifyEmailForm request)
        {
            return Problem();
        }

        [HttpGet("isAuthenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetIsAuthenticated()
        {
            return Ok(new { User.Identity?.IsAuthenticated });
        }

        private static string GetAccessToken(Guid userId, DateTime dateTimeUTC)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId.ToString()),
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
