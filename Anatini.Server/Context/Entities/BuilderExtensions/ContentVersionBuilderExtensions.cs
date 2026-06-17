using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ContentVersionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ContentVersion> contentVersionBuilder)
        {
            contentVersionBuilder.ToTable("content_versions");

            contentVersionBuilder.HasKey(contentVersion => new { contentVersion.ContentId, contentVersion.VersionNumber });

            contentVersionBuilder.Property(contentVersion => contentVersion.ContentId).Has(order: 0);
            contentVersionBuilder.Property(contentVersion => contentVersion.VersionNumber).Has(order: 1);
            contentVersionBuilder.Property(contentVersion => contentVersion.Article)!.Has(order: 2);
            contentVersionBuilder.Property(contentVersion => contentVersion.ConcurrencyStamp).Has(order: 3);
            contentVersionBuilder.Property(contentVersion => contentVersion.CreatedAtUtc).Has(order: 4);
            contentVersionBuilder.Property(contentVersion => contentVersion.UpdatedAtUtc).Has(order: 5);

            contentVersionBuilder.HasOneWithMany(contentVersion => contentVersion.Content, content => content.Versions, contentVersion => contentVersion.ContentId, DeleteBehavior.Cascade);
        }
    }
}
