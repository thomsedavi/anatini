using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class WorkVersionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<WorkVersion> workVersionBuilder)
        {
            workVersionBuilder.ToTable("work_versions");

            workVersionBuilder.HasKey(workVersion => new { workVersion.WorkId, workVersion.VersionNumber });

            workVersionBuilder.Property(workVersion => workVersion.WorkId).Has(order: 0);
            workVersionBuilder.Property(workVersion => workVersion.VersionNumber).Has(order: 1);
            workVersionBuilder.Property(workVersion => workVersion.Article)!.Has(order: 2);
            workVersionBuilder.Property(workVersion => workVersion.ConcurrencyStamp).Has(order: 3);
            workVersionBuilder.Property(workVersion => workVersion.CreatedAtUtc).Has(order: 4);
            workVersionBuilder.Property(workVersion => workVersion.UpdatedAtUtc).Has(order: 5);

            workVersionBuilder.HasOneWithMany(workVersion => workVersion.Work, activity => activity.Versions, workVersion => workVersion.WorkId, DeleteBehavior.Cascade);
        }
    }
}
