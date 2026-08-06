using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostImage> postImageBuilder)
        {
            postImageBuilder.ToTable("post_images");

            postImageBuilder.HasKey(postImage => new { postImage.PostId, postImage.Handle });

            postImageBuilder.Property(postImage => postImage.PostId).Has(order: 0);
            postImageBuilder.Property(postImage => postImage.Handle)!.Has(maxLength: 255, order: 1);
            postImageBuilder.Property(postImage => postImage.BlobName)!.Has(maxLength: 255, order: 2);
            postImageBuilder.Property(postImage => postImage.BlobContainerName)!.Has(maxLength: 255, order: 3);
            postImageBuilder.Property(postImage => postImage.AltText).Has(maxLength: 511, order: 4);
            postImageBuilder.Property(postImage => postImage.CreatedAtUtc).Has(order: 5);
            postImageBuilder.Property(postImage => postImage.UpdatedAtUtc).Has(order: 6);

            postImageBuilder.HasOneWithMany(postImage => postImage.Post, activity => activity.Images, postImage => postImage.PostId, DeleteBehavior.Restrict);
        }
    }
}
