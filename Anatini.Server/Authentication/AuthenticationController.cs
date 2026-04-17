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
    public class AuthenticationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : AnatiniControllerBase(context, userManager)
    {
        [HttpPost("email")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostEmail([FromForm] EmailForm emailForm) => await UsingContextAsync(async context =>
        {
            try
            {
                context.AddUserEmail(emailForm.Email, UserManager.NormalizeEmail(emailForm.Email));
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbUpdateException) when (dbUpdateException.InnerException is PostgresException postgresException && postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                // do not confirm that a user with this email already exists
                return NoContent();
            }

            return NoContent();
        });

        [HttpPost("sign-up")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSignUp([FromForm] SignUpForm signUpForm) => await UsingContextAsync(async (context) =>
        {
            var userEmail = await context.UserEmails.FirstOrDefaultAsync(userEmail => userEmail.NormalizedEmail.Equals(UserManager.NormalizeEmail(signUpForm.Email)));

            if (userEmail == null || userEmail.EmailConfirmed)
            {
                return NotFound();
            }

            if (userEmail.ConfirmationCode != signUpForm.ConfirmationCode)
            {
                context.UserEmails.Remove(userEmail);
                await context.SaveChangesAsync();
                return NotFound();
            }

            var (identityResult, user) = await UserManager.AddUserAsync(NormalizeHandle(signUpForm.Handle), signUpForm.Name, signUpForm.Password, signUpForm.Visibility, userEmail);

            if (!identityResult.Succeeded)
            {
                // TODO log errors
                return Problem();
            }

            await signInManager.SignInAsync(user, isPersistent: signUpForm.IsPersistent ?? false);

            return Ok();
        });

        [Authorize]
        [HttpPost("sign-out")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PostSignOut()
        {
            await signInManager.SignOutAsync();

            // TODO
            //return RedirectToAction("", "");
            return NoContent();
        }

        // TODO when changing password call UserManager.UpdateSecurityStampAsync(user)

        [HttpPost("sign-in")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostSignIn([FromForm] SignInForm signInForm) => await UsingContextAsync(async (context) =>
        {
            var user = await context.GetUserAsync(UserManager.NormalizeEmail(signInForm.Email));

            if (user == null)
            {
                return Unauthorized();
            }

            var signInResult = await signInManager.PasswordSignInAsync(user, signInForm.Password, signInForm.IsPersistent ?? false, lockoutOnFailure: true);

            if (!signInResult.Succeeded)
            {
                return Unauthorized();
            }

            return Ok();
        });

        [Authorize]
        [HttpPost("email/verify")]
        [Consumes(MediaTypeNames.Multipart.FormData)]
        [Produces(MediaTypeNames.Application.Json)]
        public IActionResult PostVerifyEmail([FromForm] VerifyEmailForm verifyEmailForm)
        {
            return Problem();
        }

        [HttpGet("is-authenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIsAuthenticated()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var isTrusted = User.HasClaim(c => c.Type == "http://anatini.com/claims/istrusted" && c.Value == "true");

                return Ok(new IsAuthenticatedResponse { IsAuthenticated = User.Identity?.IsAuthenticated ?? false, IsTrusted = isTrusted });
            }
            else
            {
                return Ok(new IsAuthenticatedResponse { IsAuthenticated = false });
            }
        }
    }
}
