using System.Security.Claims;
using Anatini.Server.Context.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Anatini.Server.Authorization
{
    public sealed class WriteUserRequirement : IAuthorizationRequirement { }

    public sealed class WriteUserHandler : AuthorizationHandler<WriteUserRequirement, ApplicationUser>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WriteUserRequirement requirement, ApplicationUser user)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid sourceUserId))
            {
                context.Fail();
                return;
            };

            if (sourceUserId == user.Id)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return;
        }
    }
}
