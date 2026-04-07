using Anatini.Server.Enums;
using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<(IdentityResult, ApplicationUser)> AddUserAsync(this UserManager<ApplicationUser> userManager, string handle, string name, string normalizedHandle, string password, Visibility visibility, ApplicationUserEmail userEmail)
        {
            var id = Guid.CreateVersion7();

            userEmail.ConfirmationCode = null;
            userEmail.EmailConfirmed = true;

            var userHandle = new ApplicationUserHandle
            {
                UserId = id,
                Handle = handle,
                NormalizedHandle = normalizedHandle
            };

            var user = new ApplicationUser
            {
                Id = id,
                Email = userEmail.Email,
                UserName = userEmail.Email,
                Handle = handle,
                NormalizedHandle = normalizedHandle,
                Name = name,
                Visibility = visibility,
                Handles = [userHandle],
                Emails = [userEmail]
            };

            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);

            var identityResult = await userManager.CreateAsync(user);

            return (identityResult, user);
        }
    }
}
