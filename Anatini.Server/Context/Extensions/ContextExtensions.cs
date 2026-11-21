using System.Security.Claims;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Extensions
{
    public static class ContextExtensions
    {
        public static async Task<ApplicationUser?> GetUserAsync(this ApplicationDbContext context, string normalizedEmail)
        {
            return await context.Users.FirstOrDefaultAsync(user => user.Emails.Any(userEmail => userEmail.NormalizedEmail.Equals(normalizedEmail)));
        }

        public static async Task<bool> IsTrusted(this ApplicationDbContext context, ClaimsPrincipal user)
        {
            if (user.HasClaim(claim => claim.Type == "http://anatini.com/claims/istrusted" && claim.Value == "true"))
            {
                return true;
            }
            else
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid targetUserId))
                {
                    return false;
                }
                else
                {
                    bool hasReceivedTrust = await context.UserUserRelationships.Where(userTrust => userTrust.TargetUserId == targetUserId && userTrust.Label == UserUserRelationshipLabel.IsTrusted).Take(2).CountAsync() >= 2;

                    if (hasReceivedTrust)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
