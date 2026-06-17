using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class SpaceImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<SpaceImage> spaceImageBuilder)
        {
            spaceImageBuilder.ToTable("space_images");

            spaceImageBuilder.HasKey(spaceImage => new { spaceImage.SpaceId, spaceImage.Handle });

            spaceImageBuilder.Property(spaceImage => spaceImage.SpaceId).Has(order: 0);
            spaceImageBuilder.Property(spaceImage => spaceImage.Handle)!.Has(maxLength: 256, order: 1);
            spaceImageBuilder.Property(spaceImage => spaceImage.BlobName)!.Has(maxLength: 64, order: 2);
            spaceImageBuilder.Property(spaceImage => spaceImage.BlobContainerName)!.Has(maxLength: 16, order: 3);
            spaceImageBuilder.Property(spaceImage => spaceImage.AltText).Has(maxLength: 512, order: 4);
            spaceImageBuilder.Property(spaceImage => spaceImage.CreatedAtUtc).Has(order: 5);
            spaceImageBuilder.Property(spaceImage => spaceImage.UpdatedAtUtc).Has(order: 6);

            spaceImageBuilder.HasOneWithMany(spaceImage => spaceImage.Space, space => space.Images, spaceImage => spaceImage.SpaceId, DeleteBehavior.Restrict);
        }
    }
}
