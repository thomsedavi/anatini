using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class WorkBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Work> workBuilder)
        {
            workBuilder.ToTable("works");

            workBuilder.Property(work => work.Id).Has(order: 0);
        }
    }
}
