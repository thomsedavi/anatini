using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostImage> postImageBuilder)
        {
            postImageBuilder.ToTable("post_images");

            postImageBuilder.HasKey(postImage => postImage.Id);

            postImageBuilder.Property(postImage => postImage.AltText).HasMaxLength(256);
            postImageBuilder.Property(postImage => postImage.CreatedAtUtc).HasColumnType("timestamp with time zone");
            postImageBuilder.Property(postImage => postImage.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            postImageBuilder.Property(postImage => postImage.BlobName).HasMaxLength(64);
            postImageBuilder.Property(postImage => postImage.BlobContainerName).HasMaxLength(16);

            postImageBuilder.HasOne(postImage => postImage.Post).WithMany(post => post.Images).HasForeignKey(postImage => postImage.PostId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
