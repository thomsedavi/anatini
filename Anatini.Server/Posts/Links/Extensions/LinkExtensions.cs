using Anatini.Server.Context.Entities;
using Anatini.Server.Dtos;
using Anatini.Server.Enums;
using Anatini.Server.Images.Services;
using Anatini.Server.Spaces.Extensions;
using Anatini.Server.Users.Extensions;

namespace Anatini.Server.Posts.Links.Extensions
{
    public static class LinkExtensions
    {
        public static async Task<LinkDto> ToLinkDtoAsync(this Post link, string? linkHandle = null, IBlobService? blobService = null)
        {
            return new LinkDto
            {
                Id = link.Id,
                UserHeader = link.User != null ? await link.User.ToUserHeaderDtoAsync(blobService) : null,
                SpaceHeader = link.Space != null ? await link.Space.ToSpaceHeaderDto(blobService) : null,
                Handle = linkHandle,
                Url = link.Url ?? throw new InvalidOperationException("Url of Link was unexpectedly null"),
                Article = link.Article ?? "<article></article>",
                PublishedAtNz = link.PublishedAtNz,
                HasBookmarked = link.UserEdges.Any(userEdge => userEdge.Label == UserPostEdgeLabel.HasBookmarked),
                HasDismissed = link.UserEdges.Any(userEdge => userEdge.Label == UserPostEdgeLabel.HasDismissed),
                HasStarred = link.UserEdges.Any(userEdge => userEdge.Label == UserPostEdgeLabel.HasStarred)
            };
        }

        public static LinkEditDto ToLinkEditDto(this Post link, string? handle = null)
        {
            return new LinkEditDto
            {
                Id = link.Id,
                Handle = handle,
                Url = link.Url ?? throw new InvalidOperationException("Url of Link was unexpectedly null"),
                Article = link.Article ?? "<article></article>",
                Visibility = link.Visibility.ToString(),
                PublishedAtNz = link.PublishedAtNz
            };
        }
    }
}
