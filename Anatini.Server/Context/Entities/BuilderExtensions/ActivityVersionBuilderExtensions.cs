using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ActivityVersionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ActivityVersion> activityVersionBuilder)
        {
            activityVersionBuilder.ToTable("activity_versions");

            activityVersionBuilder.HasKey(activityVersion => new { activityVersion.ActivityId, activityVersion.VersionNumber });

            activityVersionBuilder.Property(activityVersion => activityVersion.ActivityId).Has(order: 0);
            activityVersionBuilder.Property(activityVersion => activityVersion.VersionNumber).Has(order: 1);
            activityVersionBuilder.Property(activityVersion => activityVersion.Article)!.Has(order: 2);
            activityVersionBuilder.Property(activityVersion => activityVersion.ConcurrencyStamp).Has(order: 3);
            activityVersionBuilder.Property(activityVersion => activityVersion.CreatedAtUtc).Has(order: 4);
            activityVersionBuilder.Property(activityVersion => activityVersion.UpdatedAtUtc).Has(order: 5);

            activityVersionBuilder.HasOneWithMany(activityVersion => activityVersion.Activity, activity => activity.Versions, activityVersion => activityVersion.ActivityId, DeleteBehavior.Cascade);
        }
    }
}
