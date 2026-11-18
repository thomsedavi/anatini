using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class WorkVersionBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<WorkVersion> workVersionBuilder)
        {
            workVersionBuilder.ToTable("work_versions");

            workVersionBuilder.Property(workVersion => workVersion.Id).Has(order: 0);
        }
    }
}
