using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class SpaceBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Space> spaceBuilder)
        {
            spaceBuilder.ToTable("spaces");

            spaceBuilder.HasKey(space => space.Id);

            spaceBuilder.Property(space => space.Id).Has(order: 0);
            spaceBuilder.Property(space => space.Handle)!.Has(maxLength: 256, order: 1);
            spaceBuilder.Property(space => space.Name)!.Has(maxLength: 256, order: 2);
            spaceBuilder.Property(space => space.Visibility).Has(order: 3);
            spaceBuilder.Property(space => space.About).Has(maxLength: 512, order: 4);
            spaceBuilder.Property(space => space.CreatedAtUtc).Has(order: 5);
            spaceBuilder.Property(space => space.UpdatedAtUtc).Has(order: 6);

            spaceBuilder.HasIndex(space => space.Handle).IsUnique();
        }
    }
}
