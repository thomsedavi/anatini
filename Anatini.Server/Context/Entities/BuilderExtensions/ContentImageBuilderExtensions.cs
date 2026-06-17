using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ContentImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ContentImage> contentImageBuilder)
        {
            contentImageBuilder.ToTable("content_images");

            contentImageBuilder.HasKey(contentImage => new { contentImage.ContentId, contentImage.Handle });

            contentImageBuilder.Property(contentImage => contentImage.ContentId).Has(order: 0);
            contentImageBuilder.Property(contentImage => contentImage.Handle)!.Has(maxLength: 256, order: 1);
            contentImageBuilder.Property(contentImage => contentImage.BlobName)!.Has(maxLength: 64, order: 2);
            contentImageBuilder.Property(contentImage => contentImage.BlobContainerName)!.Has(maxLength: 16, order: 3);
            contentImageBuilder.Property(contentImage => contentImage.AltText).Has(maxLength: 512, order: 4);
            contentImageBuilder.Property(contentImage => contentImage.CreatedAtUtc).Has(order: 5);
            contentImageBuilder.Property(contentImage => contentImage.UpdatedAtUtc).Has(order: 6);

            contentImageBuilder.HasOneWithMany(contentImage => contentImage.Content, content => content.Images, contentImage => contentImage.ContentId, DeleteBehavior.Restrict);
        }
    }
}
