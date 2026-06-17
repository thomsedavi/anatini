using Anatini.Server.Enums;

namespace Anatini.Server.Context.Entities.Extensions
{
    public static class SpaceContextExtensions
    {
        public static Space AddSpace(this ApplicationDbContext context, Guid userId, string handle, string name, Visibility visibility)
        {
            var spaceId = Guid.CreateVersion7();
            var utcNow = DateTime.UtcNow;

            var spaceHandle = new SpaceHandle
            {
                Id = Guid.CreateVersion7(),
                SpaceId = spaceId,
                Handle = handle,
                CreatedAtUtc = utcNow
            };

            var userSpaceEdge = new ApplicationUserSpaceEdge
            {
                SourceUserId = userId,
                TargetSpaceId = spaceId,
                Label = UserSpaceEdgeLabel.Owner,
                CreatedAtUtc = utcNow
            };

            var space = new Space
            {
                Id = spaceId,
                Name = name,
                Handle = handle,
                Visibility = visibility,
                Handles = [spaceHandle],
                UserEdges = [userSpaceEdge],
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(space);

            return space;
        }
    }
}
