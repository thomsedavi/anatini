using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Images.Services;
using Anatini.Server.Utils;

namespace Anatini.Server.Works
{
    public static class WorkExtensions
    {
        public static async Task<WorkDto> ToWorkDtoAsync(this Work work, Guid? userId, IBlobService? blobService = null, ApplicationUser? workUser = null, Space? workSpace = null)
        {
            workUser ??= work.User;
            workSpace ??= work.Space;

            var (isBookmarked, isHidden, isLiked, isCollected, isOwned, ownerHeader) = await ContentHelper.GetUserContentDetails(work, userId, workUser, workSpace, blobService);

            return new WorkDto
            {
                Id = work.Id,
                OwnerHeader = ownerHeader,
                Handle = work.Handle,
                Title = work.Title ?? throw new InvalidOperationException(),
                Body = work.Body,
                Visibility = work.Visibility.ToString(),
                Status = work.Status.ToString(),
                PublishedAtNz = work.PublishedAtUtc?.ConvertUtcToNz(),
                IsScheduled = work.PublishedAtUtc.HasValue ? work.PublishedAtUtc > DateTime.UtcNow : null,
                IsBookmarked = isBookmarked,
                IsHidden = isHidden,
                IsLiked = isLiked,
                IsCollected = isCollected,
                IsOwned = isOwned
            };
        }
    }
}
