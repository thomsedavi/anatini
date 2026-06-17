using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class SpaceHandleBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<SpaceHandle> spaceHandleBuilder)
        {
            spaceHandleBuilder.ToTable("space_handles");

            spaceHandleBuilder.HasKey(spaceHandle => spaceHandle.Id);

            spaceHandleBuilder.Property(spaceHandle => spaceHandle.Id).Has(order: 0);
            spaceHandleBuilder.Property(spaceHandle => spaceHandle.SpaceId).Has(order: 1);
            spaceHandleBuilder.Property(spaceHandle => spaceHandle.Handle)!.Has(maxLength: 256, order: 2);
            spaceHandleBuilder.Property(spaceHandle => spaceHandle.CreatedAtUtc).Has(order: 3);

            spaceHandleBuilder.HasOneWithMany(spaceHandle => spaceHandle.Space, space => space.Handles, spaceHandle => spaceHandle.SpaceId, DeleteBehavior.Cascade);

            spaceHandleBuilder.HasIndex(spaceHandle => spaceHandle.Handle).IsUnique();
        }
    }
}
