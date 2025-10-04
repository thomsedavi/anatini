using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Anatini.Server.Context;
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
            async Task<IActionResult> userContextFunction(User user, AnatiniContext context)
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
                            context.AddUserInvite(inviteCode, user.Id, eventData.DateOnlyNZNow);
                            await context.SaveChangesAsync();
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
                    context.Update(user);

                    context.AddUserEvent(user.Id, EventType.InviteCreated, eventData);

                    return Created("?", user.ToUserEditDto());
                }
            }

            return await UsingUserContextAsync(userContextFunction);
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
                string userId;

                if (form.InviteCode == "zzzzzzzz")
                {
                    userId = IdGenerator.Get();
                }
                else
                {
                    var invite = await context.UserInvites.FindAsync(form.InviteCode);

                    if (invite == null)
                    {
                        return Ok();
                    }

                    if (invite.EmailAddress != null)
                    {
                        var email = await context.UserEmails.FindAsync(invite.EmailAddress);

                        if (email != null)
                        {
                            // Delete existing email result
                            // TODO maybe just update existing email with new details?
                            context.Remove(email);
                        }
                    }

                    invite.EmailAddress = form.EmailAddress;
                    context.Update(invite);

                    userId = invite.NewUserId;
                    eventData.Add("invitedByUserId", invite.UserId);
                }

                context.AddUserEmail(form.EmailAddress, userId);
                context.AddUserEvent(userId, EventType.EmailCreated, eventData);

                return Ok();
            }

            IActionResult onDbUpdateException(DbUpdateException dbUpdateException, IActionResult defaultResult)
            {
                if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
                {
                    // neither confirm nor deny that this email address already exists
                    return Ok();
                }
                else
                {
                    return defaultResult;
                }
            }
            ;

            return await UsingContextAsync(contextFunctionAsync, onDbUpdateException);
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

                var email = await context.UserEmails.FindAsync(newUser.EmailAddress);

                if (email == null)
                {
                    return NotFound();
                }

                if (email.Verified)
                {
                    return NotFound();
                }

                newUser.Id = email.UserId;

                if (email.VerificationCode != newUser.VerificationCode)
                {
                    context.Remove(email);
                    context.AddUserEvent(newUser.Id, EventType.VerificationBad, eventData);

                    return NotFound();
                }

                var refreshToken = TokenGenerator.Get;

                var userSlug = newUser.CreateSlug();
                var user = newUser.Create(email, refreshToken, eventData);

                context.AddRange(userSlug, user);

                email.Verified = true;
                email.VerificationCode = null;

                context.Update(email);
                context.AddUserEvent(user.Id, EventType.UserCreated, eventData);

                if (email.InvitedByUserId != null)
                {
                    var invitedByUserId = email.InvitedByUserId;

                    var invitedByUser = (await context.Users.FindAsync(invitedByUserId));

                    invitedByUser!.Invites!.First(invite => invite.Code == email.InviteCode).Used = true;

                    context.Update(invitedByUser);

                    context.AddUserToUserRelationships(newUser.Id, invitedByUserId, UserToUserRelationshipType.InvitedBy, UserToUserRelationshipType.Trusts, UserToUserRelationshipType.TrustedBy);
                    context.AddUserToUserRelationships(invitedByUserId, newUser.Id, UserToUserRelationshipType.Invites, UserToUserRelationshipType.Trusts, UserToUserRelationshipType.TrustedBy);

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
            async Task<IActionResult> contextFunctionAsync(AnatiniContext context)
            {
                var eventData = new EventData(HttpContext).Add("emailAddress", form.EmailAddress);

                var user = await context.VerifyPassword(form.EmailAddress, form.Password);

                if (user == null)
                {
                    return NotFound();
                }

                context.AddUserEvent(user.Id, EventType.LoginOk, eventData);

                var refreshToken = TokenGenerator.Get;

                user.AddSession(refreshToken, eventData);
                context.Update(user);

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
            async Task<IActionResult> userFunction(User user)
            {
                return await Task.FromResult(Ok(user.ToUserEditDto()));
            }

            return await UsingUserAsync(userFunction);
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
        public IActionResult GetIsAuthenticated()
        {
            return Ok(new { User.Identity?.IsAuthenticated });
        }

        private static string GetAccessToken(string userId, DateTime dateTimeUTC)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userId),
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
