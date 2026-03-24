using System.Net.Mime;
using Anatini.Server.Authentication.Responses;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.Extensions;
using Anatini.Server.Context.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Anatini.Server.Authentication
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : AnatiniControllerBase(context)
    {
        [HttpPost("email")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEmail([FromForm] EmailForm form) => await UsingContextAsync(async context =>
        {
            try
            {
                context.AddUserEmailAsync(form.Email, userManager.NormalizeEmail(form.Email));
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                // do not confirm that a user with this email already exists
                return NoContent();
            }

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
            var userEmail = await context.UserEmails.FirstOrDefaultAsync(u => u.NormalizedEmail.Equals(userManager.NormalizeEmail(newUser.EmailAddress)));

            if (userEmail == null || userEmail.EmailConfirmed)
            {
                return NotFound();
            }

            if (userEmail.ConfirmationCode != newUser.ConfirmationCode)
            {
                await context.RemoveAsync(userEmail);
                return NotFound();
            }

            var user = await userManager.AddUserAsync(newUser.DisplayName, newUser.UserName, userManager.NormalizeName(newUser.UserName), newUser.Password, newUser.Visibility, userEmail);

            await signInManager.SignInAsync(user, isPersistent: newUser.IsPersistent ?? false);

            return Ok(new IsAuthenticatedResponse { IsAuthenticated = true });
        });

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PostLogout()
        {
            await signInManager.SignOutAsync();
            return NoContent();
        }

        [HttpPost("login")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostLogin([FromForm] LoginForm form) => await UsingContextAsync(async context =>
        {
            //var eventData = new EventData(HttpContext).Add("emailAddress", form.EmailAddress);
            //
            //var user = await context.VerifyPassword(form.EmailAddress, form.Password);
            //
            //if (user == null)
            //{
            //    return NotFound();
            //}
            //
            //await context.AddUserEventAsync(user.Id, EventType.LoginOk, eventData);
            //
            //var refreshToken = TokenGenerator.Get;
            //
            //user.AddSession(refreshToken, eventData);
            //await context.UpdateAsync(user);
            //
            //var accessTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetAccessTokenExpiry());
            //var refreshTokenCookie = GetTokenCookie(user.Id, eventData.DateTimeUtc.GetRefreshTokenExpiry(), refreshToken);
            //
            //AppendCookie(Constants.AccessToken, accessTokenCookie, eventData.DateTimeUtc.GetAccessTokenExpiry());
            //AppendCookie(Constants.RefreshToken, refreshTokenCookie, eventData.DateTimeUtc.GetRefreshTokenExpiry());
            //
            //return Ok(new IsAuthenticatedResponse { IsAuthenticated = true, ExpiresUtc = eventData.DateTimeUtc.GetAccessTokenExpiry() });
            return Ok();
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
            return Ok(new IsAuthenticatedResponse{ IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
        }
    }
}
