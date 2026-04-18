namespace Anatini.Server.Context.Entities.Extensions
{
    public static class PostImageContextExtensions
    {
        public static PostImage AddPostImage(this ApplicationDbContext context, Guid id, Guid postId, string handle, string blobContainerName, string blobName, string? altText = null)
        {
            var utcNow = DateTime.UtcNow;

            var postImage = new PostImage
            {
                Id = id,
                PostId = postId,
                Handle = handle,
                BlobContainerName = blobContainerName,
                BlobName = blobName,
                AltText = altText,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow
            };

            context.Add(postImage);

            return postImage;
        }
    }
}
