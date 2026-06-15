using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostVersionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostVersion> postVersionBuilder)
        {
            postVersionBuilder.ToTable("post_versions");

            postVersionBuilder.HasKey(postVersion => new { postVersion.PostId, postVersion.VersionNumber });

            postVersionBuilder.Property(postVersion => postVersion.PostId).Has(order: 0);
            postVersionBuilder.Property(postVersion => postVersion.VersionNumber).Has(order: 1);
            postVersionBuilder.Property(postVersion => postVersion.Article)!.Has(order: 2);
            postVersionBuilder.Property(postVersion => postVersion.ConcurrencyStamp).Has(order: 3);
            postVersionBuilder.Property(postVersion => postVersion.CreatedAtUtc).Has(order: 4);
            postVersionBuilder.Property(postVersion => postVersion.UpdatedAtUtc).Has(order: 5);

            postVersionBuilder.HasOneWithMany(postVersion => postVersion.Post, post => post.Versions, postVersion => postVersion.PostId, DeleteBehavior.Cascade);
        }
    }
}
