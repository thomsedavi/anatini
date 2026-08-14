using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Works.Websites.Extensions
{
    public static class WebsiteExtensions
    {
        public static async Task<WebsiteDto> ToWebsiteDtoAsync(this Work work, string? websiteHandle = null, IBlobService? blobService = null)
        {
            return new WebsiteDto
            {
                Id = work.Id,
                UserHeader = work.User != null ? await work.User.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = work.Space != null ? await work.Space.ToSpaceHeaderDto(blobService) : null,
                Handle = websiteHandle,
                Name = work.Name,
                Article = work.Article,
                Url = work.Url,
                PublishedAtNz = work.PublishedAtNz,
                HasBookmarked = work.UserEdges.Any(userEdge => userEdge.Label == UserWorkEdgeLabel.HasBookmarked),
                HasDismissed = work.UserEdges.Any(userEdge => userEdge.Label == UserWorkEdgeLabel.HasDismissed),
                HasStarred = work.UserEdges.Any(userEdge => userEdge.Label == UserWorkEdgeLabel.HasStarred)
            };
        }
    }
}
