using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ActivityImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ActivityImage> activityImageBuilder)
        {
            activityImageBuilder.ToTable("activity_images");

            activityImageBuilder.HasKey(activityImage => new { activityImage.ActivityId, activityImage.Handle });

            activityImageBuilder.Property(activityImage => activityImage.ActivityId).Has(order: 0);
            activityImageBuilder.Property(activityImage => activityImage.Handle)!.Has(maxLength: 255, order: 1);
            activityImageBuilder.Property(activityImage => activityImage.BlobName)!.Has(maxLength: 255, order: 2);
            activityImageBuilder.Property(activityImage => activityImage.BlobContainerName)!.Has(maxLength: 255, order: 3);
            activityImageBuilder.Property(activityImage => activityImage.AltText).Has(maxLength: 511, order: 4);
            activityImageBuilder.Property(activityImage => activityImage.CreatedAtUtc).Has(order: 5);
            activityImageBuilder.Property(activityImage => activityImage.UpdatedAtUtc).Has(order: 6);

            activityImageBuilder.HasOneWithMany(activityImage => activityImage.Activity, activity => activity.Images, activityImage => activityImage.ActivityId, DeleteBehavior.Restrict);
        }
    }
}
