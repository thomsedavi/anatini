using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Post> postBuilder)
        {
            postBuilder.ToTable("posts");

            postBuilder.HasKey(post => post.Id);

            postBuilder.Property(post => post.Id).Has(order: 0);
            postBuilder.Property(post => post.ChannelId).Has(order: 1);
            postBuilder.Property(post => post.Handle)!.Has(maxLength: 256, order: 2);
            postBuilder.Property(post => post.Name)!.Has(maxLength: 256, order: 3);
            postBuilder.Property(post => post.PublishedAtUtc).Has(order: 4);
            postBuilder.Property(post => post.Status).Has(order: 5);
            postBuilder.Property(post => post.Visibility).Has(order: 6);
            postBuilder.Property(post => post.ConcurrencyStamp).Has(order: 7).IsConcurrencyToken();
            postBuilder.Property(post => post.CreatedAtUtc).Has(order: 8);
            postBuilder.Property(post => post.UpdatedAtUtc).Has(order: 9);

            postBuilder.HasOneWithMany(post => post.Channel, channel => channel.Posts, post => post.ChannelId, DeleteBehavior.Restrict);

            postBuilder.HasIndex(post => new { post.ChannelId, post.Handle }).IsUnique();
            postBuilder.HasIndex(post => new { post.ChannelId, post.PublishedAtUtc });
            postBuilder.HasIndex(post => post.PublishedAtUtc ).HasFilter($"{postBuilder.GetColumnName(nameof(Post.Status))} = {(int)PostStatus.Published}").HasDatabaseName("ix_published_posts_date_nz");
        }
    }
}
