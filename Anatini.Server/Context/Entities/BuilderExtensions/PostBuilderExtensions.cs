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
            postBuilder.Property(post => post.NormalizedHandle)!.Has(maxLength: 256, order: 3);
            postBuilder.Property(post => post.DateNZ).Has(order: 4);
            postBuilder.Property(post => post.Status).Has(order: 5);
            postBuilder.Property(post => post.Visibility).Has(order: 6);
            postBuilder.Property(post => post.DraftVersion)!.Has(order: 7).HasConversion();
            postBuilder.Property(post => post.PublishedVersion).Has(order: 8).HasConversion();
            postBuilder.Property(post => post.ConcurrencyStamp).Has(order: 9).IsConcurrencyToken();
            postBuilder.Property(post => post.CreatedAtUtc).Has(order: 10);
            postBuilder.Property(post => post.UpdatedAtUtc).Has(order: 11);

            postBuilder.HasOneWithMany(post => post.Channel, channel => channel.Posts, post => post.ChannelId, DeleteBehavior.Restrict);

            postBuilder.HasIndex(post => new { post.ChannelId, post.NormalizedHandle }).IsUnique();
            postBuilder.HasIndex(post => new { post.ChannelId, post.DateNZ });
            postBuilder.HasIndex(post => post.DateNZ ).HasFilter($"{postBuilder.GetColumnName(nameof(Post.Status))} = {(int)PostStatus.Published}").HasDatabaseName("ix_published_posts_date_nz");
        }
    }
}
