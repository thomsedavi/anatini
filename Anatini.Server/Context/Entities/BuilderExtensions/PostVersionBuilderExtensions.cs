using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostVersionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<PostVersion> postVersionBuilder)
        {
            postVersionBuilder.ToTable("post_versions");

            postVersionBuilder.HasKey(postVersion => postVersion.Id);

            postVersionBuilder.Property(postVersion => postVersion.Id).Has(order: 0);
            postVersionBuilder.Property(postVersion => postVersion.PostId).Has(order: 1);
            postVersionBuilder.Property(postVersion => postVersion.Handle)!.Has(maxLength: 256, order: 2);
            postVersionBuilder.Property(postVersion => postVersion.Article)!.Has(order: 4);

            postVersionBuilder.HasOneWithMany(postVersion => postVersion.Post, post => post.Versions, postVersion => postVersion.PostId, DeleteBehavior.Cascade);

            postVersionBuilder.HasIndex(postVersion => new { postVersion.PostId, postVersion.Handle }).IsUnique();
        }
    }
}
