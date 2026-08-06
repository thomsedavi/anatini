using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ActivityBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Activity> activityBuilder)
        {
            activityBuilder.ToTable("activities", tableBuilder => tableBuilder.HasCheckConstraint("ck_activities_user_id_xor_space_id", $"({activityBuilder.GetColumnName(nameof(Activity.UserId))} IS NULL AND {activityBuilder.GetColumnName(nameof(Activity.SpaceId))} IS NOT NULL) OR ({activityBuilder.GetColumnName(nameof(Activity.SpaceId))} IS NULL AND {activityBuilder.GetColumnName(nameof(Activity.UserId))} IS NOT NULL)"));

            activityBuilder.HasKey(activity => activity.Id);

            activityBuilder.Property(activity => activity.Id).Has(order: 0);
            activityBuilder.Property(activity => activity.UserId).Has(order: 1);
            activityBuilder.Property(activity => activity.SpaceId).Has(order: 2);
            activityBuilder.Property(activity => activity.Handle)!.Has(maxLength: 255, order: 3);
            activityBuilder.Property(activity => activity.Type).Has(order: 4);
            activityBuilder.Property(activity => activity.Status).Has(order: 5);
            activityBuilder.Property(activity => activity.PublishedAtUtc).Has(order: 6);
            activityBuilder.Property(activity => activity.Visibility).Has(order: 7);
            activityBuilder.Property(activity => activity.Name).Has(maxLength: 255, order: 8);
            activityBuilder.Property(activity => activity.Article).Has(order: 9);
            activityBuilder.Property(activity => activity.Url).Has(maxLength: 2047, order: 10);
            activityBuilder.Property(activity => activity.CurrentVersionNumber).Has(order: 11);
            activityBuilder.Property(activity => activity.ConcurrencyStamp)!.Has(order: 12).IsConcurrencyToken();
            activityBuilder.Property(activity => activity.CreatedAtUtc).Has(order: 13);
            activityBuilder.Property(activity => activity.UpdatedAtUtc).Has(order: 14);

            activityBuilder.HasOneWithMany(activity => activity.User, user => user.Activities, activity => activity.UserId, DeleteBehavior.Restrict, required: false);
            activityBuilder.HasOneWithMany(activity => activity.Space, space => space.Activities, activity => activity.SpaceId, DeleteBehavior.Restrict, required: false);

            activityBuilder.HasIndex(activity => new { activity.UserId, activity.Type, activity.Handle }).IsUnique().HasFilter($"{activityBuilder.GetColumnName(nameof(Activity.UserId))} IS NOT NULL");
            activityBuilder.HasIndex(activity => new { activity.SpaceId, activity.Type, activity.Handle }).IsUnique().HasFilter($"{activityBuilder.GetColumnName(nameof(Activity.SpaceId))} IS NOT NULL");
            activityBuilder.HasIndex(activity => activity.PublishedAtUtc ).HasFilter($"{activityBuilder.GetColumnName(nameof(Activity.Status))} = {(int)Status.Published}").HasDatabaseName("ix_published_activities_date_nz");
        }
    }
}
