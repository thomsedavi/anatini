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

            postBuilder.Property(post => post.Handle).HasMaxLength(256);
            postBuilder.Property(post => post.NormalizedHandle).HasMaxLength(256);
            postBuilder.Property(post => post.Visibility).HasMaxLength(16);
            postBuilder.Property(post => post.CreatedAtUtc).HasColumnType("timestamp with time zone");
            postBuilder.Property(post => post.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            postBuilder.Property(post => post.ConcurrencyStamp).IsConcurrencyToken();

            postBuilder.OwnsOne(post => post.DraftVersion, builder => { builder.ToJson(); });
            postBuilder.OwnsOne(post => post.PublishedVersion, builder => { builder.ToJson(); });

            postBuilder.HasIndex(post => post.NormalizedHandle).IsUnique();
        }
    }
}
