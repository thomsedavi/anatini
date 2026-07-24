using System.Security.Claims;
using Anatini.Server.Context;
using Anatini.Server.Context.Entities;
using Anatini.Server.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Authorization
{
    public sealed class WriteSpaceRequirement : IAuthorizationRequirement { }

    public sealed class WriteSpaceHandler(ApplicationDbContext dbContext) : AuthorizationHandler<WriteSpaceRequirement, Space>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, WriteSpaceRequirement requirement, Space space)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid sourceUserId))
            {
                context.Fail();
                return;
            };

            var isOwner = await dbContext.UserSpaceEdges.AnyAsync(userSpace => userSpace.SourceUserId == sourceUserId && userSpace.TargetSpaceId == space.Id && userSpace.Label == UserSpaceEdgeLabel.Owner);

            if (isOwner)
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
