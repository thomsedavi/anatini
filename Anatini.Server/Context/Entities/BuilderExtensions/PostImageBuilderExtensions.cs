using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostImage> postImageBuilder)
        {
            postImageBuilder.ToContainer("PostImages");
            postImageBuilder.HasKey(postImage => new { postImage.PostChannelId, postImage.PostId, postImage.Id });
            postImageBuilder.HasPartitionKey(postImage => new { postImage.PostChannelId, postImage.PostId, postImage.Id });
            postImageBuilder.Property(postImage => postImage.ItemId).ToJsonProperty("id");
            postImageBuilder.Property(postImage => postImage.Id).ToJsonProperty( "Id");
            postImageBuilder.Property(postImage => postImage.ETag).ToJsonProperty("_etag");
            postImageBuilder.Property(postImage => postImage.Timestamp).ToJsonProperty("_ts");
        }
    }
}
