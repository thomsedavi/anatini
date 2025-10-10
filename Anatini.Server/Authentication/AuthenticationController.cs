using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Anatini.Server.Authentication.Responses;
using Anatini.Server.Context;
using Anatini.Server.Context.EntityExtensions;
using Anatini.Server.Enums;
using Anatini.Server.Users.Extensions;
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
        [HttpPost("invite")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostInvite()
        {
            async Task<IActionResult> userContextFunctionAsync(User user, AnatiniContext context)
            {
                var eventData = new EventData(HttpContext);

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
                            await context.AddUserInviteAsync(inviteCode, user.Id, eventData.DateOnlyNZNow);
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
                    await context.Update(user);

                    await context.AddUserEventAsync(user.Id, EventType.InviteCreated, eventData);

                    return Created("?", user.ToUserEditDto());
                }
            }

            return await UsingUserContextAsync(UserId, userContextFunctionAsync);
        }

        [HttpPost("email")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEmail([FromForm] EmailForm form)
        {
            async Task<IActionResult> contextFunctionAsync(AnatiniContext context)
            {
                var eventData = new EventData(HttpContext).Add("emailAddress", form.EmailAddress);
                Guid userId;

                if (form.InviteCode == "zzzzzzzz")
                {
                    userId = Guid.NewGuid();
                }
                else
                {
                    var invite = await context.FindAsync<UserInvite>(form.InviteCode);

                    if (invite == null)
                    {
                        return Ok();
                    }

                    if (invite.EmailAddress != null)
                    {
                        var email = await context.FindAsync<UserEmail>(invite.EmailAddress);

                        if (email != null)
                        {
                            // Delete existing email result
                            // TODO maybe just update existing email with new details?
                            await context.Remove(email);
                        }
                    }

                    invite.EmailAddress = form.EmailAddress;
                    await context.Update(invite);

                    userId = invite.NewUserId;
                    eventData.Add("invitedByUserId", invite.UserId);
                }

                // TODO make sure it's okay that the email and invite get deleted before this point
                try
                {
                    await context.AddUserEmailAsync(form.EmailAddress, userId);
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
                    {
                        return Ok();
                    }
                    else
                    {
                        throw;
                    }
                }

                await context.AddUserEventAsync(userId, EventType.EmailCreated, eventData);

                return Ok();
            }

            return await UsingContextAsync(contextFunctionAsync);
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
            async Task<IActionResult> contextFunctionAsync(AnatiniContext context)
            {
                var eventData = new EventData(HttpContext).Add("emailAddress", newUser.EmailAddress).Add("name", newUser.Name);

                var email = await context.FindAsync<UserEmail>(newUser.EmailAddress);

                if (email == null)
                {
                    return NotFound();
                }

                if (email.Verified)
                {
                    return NotFound();
                }

                var userSlug = await context.AddUserAliasAsync(newUser.Id, newUser.Slug, newUser.Name);

                newUser.Id = email.UserId;

                if (email.VerificationCode != newUser.VerificationCode)
                {
                    await context.Remove(email);
                    await context.AddUserEventAsync(newUser.Id, EventType.VerificationBad, eventData);

                    return NotFound();
                }

                var refreshToken = TokenGenerator.Get;

                var user = await context.AddUserAsync(newUser.Id, newUser.Name, newUser.Slug, newUser.Password, email.Address, refreshToken, eventData);

                email.Verified = true;
                email.VerificationCode = null;

                await context.Update(email);
                await context.AddUserEventAsync(user.Id, EventType.UserCreated, eventData);

                if (email.InvitedByUserId.HasValue)
                {
                    var invitedByUserId = email.InvitedByUserId.Value;

                    var invitedByUser = (await context.FindAsync<User>(invitedByUserId));

                    invitedByUser!.Invites!.First(invite => invite.Code == email.InviteCode).Used = true;

                    await context.Update(invitedByUser);

                    await context.AddUserToUserRelationshipsAsync(newUser.Id, invitedByUserId, UserToUserRelationshipType.InvitedBy, UserToUserRelationshipType.Trusts, UserToUserRelationshipType.TrustedBy);
                    await context.AddUserToUserRelationshipsAsync(invitedByUserId, newUser.Id, UserToUserRelationshipType.Invites, UserToUserRelationshipType.Trusts, UserToUserRelationshipType.TrustedBy);

                    email.InvitedByUserId = null;
                }

                var accessToken = GetAccessToken(user.Id, eventData.DateTimeUtc);

                AppendCookies(accessToken, refreshToken, eventData.DateTimeUtc);

                return Ok();
            }

            return await UsingContextAsync(contextFunctionAsync);
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult PostLogout()
        {
            DeleteCookie("access_token");
            DeleteCookie("refresh_token");

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
            async Task<IActionResult> contextFunctionAsync(AnatiniContext context)
            {
                var eventData = new EventData(HttpContext).Add("emailAddress", form.EmailAddress);

                var user = await context.VerifyPassword(form.EmailAddress, form.Password);

                if (user == null)
                {
                    return NotFound();
                }

                await context.AddUserEventAsync(user.Id, EventType.LoginOk, eventData);

                var refreshToken = TokenGenerator.Get;

                user.AddSession(refreshToken, eventData);
                await context.Update(user);

                var accessToken = GetAccessToken(user.Id, eventData.DateTimeUtc);

                AppendCookies(accessToken, refreshToken, eventData.DateTimeUtc);

                return Ok();
            }

            return await UsingContextAsync(contextFunctionAsync);
        }

        [Authorize]
        [HttpGet("account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserEdit()
        {
            async Task<IActionResult> userFunctionAsync(User user)
            {
                return await Task.FromResult(Ok(user.ToUserEditDto()));
            }

            return await UsingUserAsync(UserId, userFunctionAsync);
        }

        [Authorize]
        [HttpPost("email/verify")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult PostVerifyEmail([FromForm] VerifyEmailForm request)
        {
            return Problem();
        }

        [HttpGet("is-authenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIsAuthenticated()
        {
            var accessToken = CookieValue("access_token");

            if (accessToken == null)
            {
                return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);
            var exp = token.Claims.FirstOrDefault(a => a.Type == "exp")?.Value;

            DateTime expiresDateTime;

            if (exp != null && long.TryParse(exp, out long seconds))
            {
                expiresDateTime = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
            }
            else
            {
                DeleteCookie("access_token");
                DeleteCookie("refresh_token");

                return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
            }

            if (expiresDateTime < DateTime.UtcNow)
            {
                async Task<IActionResult> userFunctionAsync(User user, AnatiniContext context)
                {
                    var refreshToken = CookieValue("refresh_token");

                    var userSession = user.Sessions.FirstOrDefault(session => session.RefreshToken == refreshToken);

                    if (userSession == null)
                    {
                        DeleteCookie("access_token");
                        DeleteCookie("refresh_token");

                        return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
                    }

                    userSession.RefreshToken = TokenGenerator.Get;
                    await context.Update(user);

                    var utcNow = DateTime.UtcNow;

                    var accessToken = GetAccessToken(user.Id, utcNow);

                    AppendCookies(accessToken, userSession.RefreshToken, utcNow);

                    return Ok(new IsAuthenticatedResponse { IsAuthenticated = true });
                }

                var userId = Guid.TryParse(token.Claims.FirstOrDefault(a => a.Type == "nameid")?.Value, out Guid id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    DeleteCookie("access_token");
                    DeleteCookie("refresh_token");

                    return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
                }

                return await UsingUserContextAsync(userId, userFunctionAsync);
            }

            return Ok(new IsAuthenticatedResponse{ IsAuthenticated = true });
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
