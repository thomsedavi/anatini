using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class PostBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Post> postBuilder)
        {
            postBuilder.ToTable("posts", tableBuilder => tableBuilder.HasCheckConstraint("ck_posts_user_id_xor_space_id", $"({postBuilder.GetColumnName(nameof(Post.UserId))} IS NULL AND {postBuilder.GetColumnName(nameof(Post.SpaceId))} IS NOT NULL) OR ({postBuilder.GetColumnName(nameof(Post.SpaceId))} IS NULL AND {postBuilder.GetColumnName(nameof(Post.UserId))} IS NOT NULL)"));

            postBuilder.HasKey(post => post.Id);

            postBuilder.Property(post => post.Id).Has(order: 0);
            postBuilder.Property(post => post.UserId).Has(order: 1);
            postBuilder.Property(post => post.SpaceId).Has(order: 2);
            postBuilder.Property(post => post.Handle)!.Has(maxLength: 255, order: 3);
            postBuilder.Property(post => post.Type).Has(order: 4);
            postBuilder.Property(post => post.Status).Has(order: 5);
            postBuilder.Property(post => post.PublishedAtNz).Has(order: 6);
            postBuilder.Property(post => post.Visibility).Has(order: 7);
            postBuilder.Property(post => post.Name).Has(maxLength: 255, order: 8);
            postBuilder.Property(post => post.Article).Has(order: 9);
            postBuilder.Property(post => post.Url).Has(maxLength: 2047, order: 10);
            postBuilder.Property(post => post.CurrentVersionNumber).Has(order: 11);
            postBuilder.Property(post => post.ConcurrencyStamp)!.Has(order: 12).IsConcurrencyToken();
            postBuilder.Property(post => post.CreatedAtUtc).Has(order: 13);
            postBuilder.Property(post => post.UpdatedAtUtc).Has(order: 14);

            postBuilder.HasOneWithMany(post => post.User, user => user.Posts, post => post.UserId, DeleteBehavior.Restrict, required: false);
            postBuilder.HasOneWithMany(post => post.Space, space => space.Posts, post => post.SpaceId, DeleteBehavior.Restrict, required: false);

            postBuilder.HasIndex(post => new { post.UserId, post.Type, post.Handle }).IsUnique().HasFilter($"{postBuilder.GetColumnName(nameof(Post.UserId))} IS NOT NULL");
            postBuilder.HasIndex(post => new { post.SpaceId, post.Type, post.Handle }).IsUnique().HasFilter($"{postBuilder.GetColumnName(nameof(Post.SpaceId))} IS NOT NULL");
            postBuilder.HasIndex(post => post.PublishedAtNz).HasFilter($"{postBuilder.GetColumnName(nameof(Post.Status))} = {(int)Status.Published}").HasDatabaseName("ix_published_posts_date_nz");
        }
    }
}
