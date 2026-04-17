using System.Security.Claims;
using Anatini.Server.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Authorization
{
    public class TrustedUserRequirement : IAuthorizationRequirement { }

    public class TrustedUserHandler(ApplicationDbContext dbContext) : AuthorizationHandler<TrustedUserRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TrustedUserRequirement requirement)
        {
            if (context.User.HasClaim(claim => claim.Type == "http://anatini.com/claims/istrusted" && claim.Value == "true"))
            {
                context.Succeed(requirement);
                return;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid targetUserId))
            {
                context.Fail();
                return;
            };

            var hasReceivedTrust = await dbContext.UserTrusts.AnyAsync(userTrust => userTrust.TargetUserId == targetUserId);

            if (hasReceivedTrust)
            {
                context.Succeed(requirement);
            }
        }
    }
}
