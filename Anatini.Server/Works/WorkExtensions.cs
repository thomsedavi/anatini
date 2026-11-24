using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;
using Anatini.Server.Utils;

namespace Anatini.Server.Works
{
    public static class WorkExtensions
    {
        public static async Task<WorkDto> ToWorkDtoAsync(this Work work, Guid? userId, IBlobService? blobService = null, ApplicationUser? workUser = null, Space? workSpace = null)
        {
            workUser ??= work.User;
            workSpace ??= work.Space;

            var (isBookmarked, isDismissed, isStarred, isCollected, isOwned) = ContentHelper.GetUserContentRelationships(work, userId, workUser, workSpace);

            return new WorkDto
            {
                Id = work.Id,
                UserHeader = workUser != null ? await workUser.ToUserHeaderDtoAsync(userId.HasValue, blobService) : null,
                SpaceHeader = workSpace != null ? await workSpace.ToSpaceHeaderDto(userId.HasValue, blobService) : null,
                Handle = work.Handle,
                Title = work.Title ?? throw new InvalidOperationException(),
                Body = work.Body,
                Visibility = work.Visibility.ToString(),
                Status = work.Status.ToString(),
                PublishedAtNz = work.PublishedAtUtc?.ConvertUtcToNz(),
                IsScheduled = work.PublishedAtUtc.HasValue ? work.PublishedAtUtc > DateTime.UtcNow : null,
                IsBookmarked = isBookmarked,
                IsDismissed = isDismissed,
                IsStarred = isStarred,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }
    }
}
