using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Anatini.Server.Authentication.Extensions;
using Anatini.Server.Authentication.Responses;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
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
    [Route("api/authentication")]
    public class AuthenticationController : AnatiniControllerBase
    {
        [HttpPost("email")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEmail([FromForm] EmailForm form) => await UsingContextAsync(async context =>
        {
            var eventData = new EventData(HttpContext).Add("emailAddress", form.EmailAddress);
            var userId = Guid.NewGuid();

            try
            {
                await context.AddUserEmailAsync(form.EmailAddress, userId);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Conflict)
                {
                    return NoContent();
                }
                else
                {
                    throw;
                }
            }

            await context.AddUserEventAsync(userId, EventType.EmailCreated, eventData);

            return NoContent();
        });

        [HttpPost("signup")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSignup([FromForm] NewUser newUser) => await UsingContextAsync(async context =>
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

            newUser.Id = email.UserId;

            var userSlug = await context.AddUserAliasAsync(newUser.Id, newUser.Slug, newUser.Name, newUser.Protected);

            if (email.VerificationCode != newUser.VerificationCode)
            {
                await context.RemoveAsync(email);
                await context.AddUserEventAsync(newUser.Id, EventType.VerificationBad, eventData);

                return NotFound();
            }

            var refreshToken = TokenGenerator.Get;

            var user = await context.AddUserAsync(newUser.Id, newUser.Name, newUser.Slug, newUser.Password, email.Address, newUser.Protected, refreshToken, eventData);

            email.Verified = true;
            email.VerificationCode = null;

            await context.UpdateAsync(email);
            await context.AddUserEventAsync(user.Id, EventType.UserCreated, eventData);

            var accessTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetAccessTokenExpiry());
            var refreshTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetRefreshTokenExpiry(), refreshToken);

            AppendCookie(Constants.AccessToken, accessTokenCookie, eventData.DateTimeUtc.GetAccessTokenExpiry());
            AppendCookie(Constants.RefreshToken, refreshTokenCookie, eventData.DateTimeUtc.GetRefreshTokenExpiry());

            return Ok(new IsAuthenticatedResponse { IsAuthenticated = true, ExpiresUtc = eventData.DateTimeUtc.GetAccessTokenExpiry() });
        });

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PostLogout() => await UsingUserContextAsync(UserId, async (user, context) =>
        {
            var refreshTokenString = CookieValue(Constants.RefreshToken);

            var refreshToken = refreshTokenString?.GetClaimValue(JwtRegisteredClaimNames.Jti);

            if (refreshToken != null)
            {
                user.RemoveSession(refreshToken);
                await context.UpdateAsync(user);
            }

            DeleteCookie(Constants.AccessToken);
            DeleteCookie(Constants.RefreshToken);

            return NoContent();
        });

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var eventData = new EventData(HttpContext);

                var refreshTokenString = CookieValue(Constants.RefreshToken);

                if (refreshTokenString == null)
                {
                    return Unauthorized();
                }

                return await GetRefreshToken(refreshTokenString, eventData);
            }
            catch (Exception)
            {
                return Problem();
            }
        }

        [HttpPost("login")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostLogin([FromForm] LoginForm form) => await UsingContextAsync(async context =>
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
            await context.UpdateAsync(user);

            var accessTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetAccessTokenExpiry());
            var refreshTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetRefreshTokenExpiry(), refreshToken);

            AppendCookie(Constants.AccessToken, accessTokenCookie, eventData.DateTimeUtc.GetAccessTokenExpiry());
            AppendCookie(Constants.RefreshToken, refreshTokenCookie, eventData.DateTimeUtc.GetRefreshTokenExpiry());

            return Ok(new IsAuthenticatedResponse { IsAuthenticated = true, ExpiresUtc = eventData.DateTimeUtc.GetAccessTokenExpiry() });
        });

        [Authorize]
        [HttpPost("email/verify")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult PostVerifyEmail([FromForm] VerifyEmailForm request)
        {
            return Problem();
        }

        [HttpGet("is-authenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIsAuthenticated()
        {
            var eventData = new EventData(HttpContext);

            var refreshTokenString = CookieValue(Constants.RefreshToken);

            if (refreshTokenString == null)
            {
                return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
            }

            var accessTokenString = CookieValue(Constants.AccessToken);

            if (accessTokenString == null)
            {
                return await GetRefreshToken(refreshTokenString, eventData);
            }

            var exp = accessTokenString.GetClaimValue(JwtRegisteredClaimNames.Exp);

            DateTime expiresDateTime;

            if (exp != null && long.TryParse(exp, out long seconds))
            {
                expiresDateTime = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;
            }
            else
            {
                DeleteCookie(Constants.AccessToken);
                DeleteCookie(Constants.RefreshToken);

                return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
            }

            if (expiresDateTime < DateTime.UtcNow) // TODO refresh if ten mins remaining
            {
                return await GetRefreshToken(refreshTokenString, eventData);
            }

            return Ok(new IsAuthenticatedResponse{ IsAuthenticated = true, ExpiresUtc = expiresDateTime });
        }

        private async Task<IActionResult> GetRefreshToken(string refreshTokenString, EventData eventData)
        {
            var refreshToken = refreshTokenString?.GetClaimValue(JwtRegisteredClaimNames.Jti) ?? throw new Exception();
            var userId = Guid.TryParse(refreshTokenString.GetClaimValue(JwtRegisteredClaimNames.NameId), out Guid id) ? id : throw new Exception();

            return await UsingUserContextAsync(userId, async (user, context) =>
            {
                var userSession = user.Sessions?.FirstOrDefault(session => session.RefreshToken == refreshToken);

                if (userSession == null)
                {
                    DeleteCookie(Constants.AccessToken);
                    DeleteCookie(Constants.RefreshToken);

                    return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
                }

                userSession.RefreshToken = TokenGenerator.Get;
                await context.UpdateAsync(user);

                var accessTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetAccessTokenExpiry());
                var refreshTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetRefreshTokenExpiry(), userSession.RefreshToken);

                AppendCookie(Constants.AccessToken, accessTokenCookie, eventData.DateTimeUtc.GetAccessTokenExpiry());
                AppendCookie(Constants.RefreshToken, refreshTokenCookie, eventData.DateTimeUtc.GetRefreshTokenExpiry());

                return Ok(new IsAuthenticatedResponse { IsAuthenticated = true, ExpiresUtc = eventData.DateTimeUtc.GetAccessTokenExpiry() });
            });
        }

        private static string GetTokenCookie(Guid userId, DateTime expires, string? value = null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, userId.ToString())
            };

            if (value != null)
            {
                claims.Add(new(JwtRegisteredClaimNames.Jti, value));
            }

            var key = Encoding.UTF8.GetBytes("ItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMeItsMe2");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = "https://id.anatini.com",
                Audience = "https://api.anatini.com",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private void AppendCookie(string key, string value, DateTime expires)
        {
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = expires
            };

            Response.Cookies.Append(key, value, options);
        }

        private string? CookieValue(string key) => Request.Cookies.FirstOrDefault(cookie => cookie.Key == key).Value;
        private void DeleteCookie(string key) => Response.Cookies.Delete(key);
    }
}
