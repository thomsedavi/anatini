using Anatini.Server.Context;
using Anatini.Server.Context.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Anatini.Server.Authorization
{
    public sealed class TrustedUserRequirement : IAuthorizationRequirement { }

    public sealed class TrustedUserHandler(ApplicationDbContext dbContext) : AuthorizationHandler<TrustedUserRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TrustedUserRequirement requirement)
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
    }
}
