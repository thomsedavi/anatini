using Anatini.Server.Enums;
using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<(IdentityResult, ApplicationUser)> AddUserAsync(this UserManager<ApplicationUser> userManager, string handle, string name, string password, Visibility visibility, ApplicationUserEmail userEmail)
        {
            var userId = Guid.CreateVersion7();
            var utcNow = DateTime.UtcNow;

            userEmail.ConfirmationCode = null;
            userEmail.EmailConfirmed = true;

            var userHandle = new ApplicationUserHandle
            {
                Id = Guid.CreateVersion7(),
                UserId = userId,
                Handle = handle,
                CreatedAtUtc = utcNow
            };

            var user = new ApplicationUser
            {
                Id = userId,
                Handle = handle,
                Email = userEmail.Email,
                UserName = userEmail.Email,
                Name = name,
                Visibility = visibility,
                Handles = [userHandle],
                Emails = [userEmail],
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);

            var identityResult = await userManager.CreateAsync(user);

            return (identityResult, user);
        }
    }
}
