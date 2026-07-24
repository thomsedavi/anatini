using Anatini.Server.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Anatini.Server.Authorization
{
    public sealed class ReadRequirement : IAuthorizationRequirement { }

    public sealed class ReadHandler : AuthorizationHandler<ReadRequirement, Visibility>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReadRequirement requirement, Visibility visibility)
        {
            if (visibility == Visibility.Public)
            {
                context.Succeed(requirement);
            }
            else if (visibility == Visibility.Protected && (context.User.Identity?.IsAuthenticated ?? false))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
