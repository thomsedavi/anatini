using Anatini.Server.Context;
using Anatini.Server.Context.Extensions;
using Anatini.Server.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Anatini.Server.Authorization
{
    public sealed class ReadRequirement : IAuthorizationRequirement { }

    public sealed class ReadHandler(ApplicationDbContext dbContext) : AuthorizationHandler<ReadRequirement, Visibility>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ReadRequirement requirement, Visibility visibility)
        {
            if (visibility == Visibility.Public)
            {
                context.Succeed(requirement);
            }
            else if (visibility == Visibility.Protected)
            {
                if (await dbContext.IsTrusted(context.User))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
        }
    }
}
