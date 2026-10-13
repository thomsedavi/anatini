using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class SpaceSettingBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<SpaceSetting> spaceSettingBuilder)
        {
            spaceSettingBuilder.ToTable("space_settings");

            spaceSettingBuilder.HasKey(spaceSetting => new { spaceSetting.SpaceId, spaceSetting.Key });

            spaceSettingBuilder.Property(spaceSetting => spaceSetting.SpaceId).Has(order: 0);
            spaceSettingBuilder.Property(spaceSetting => spaceSetting.Key)!.Has(maxLength: 255, order: 1);
            spaceSettingBuilder.Property(spaceSetting => spaceSetting.Value)!.Has(maxLength: 255, order: 2);

            spaceSettingBuilder.HasOneWithMany(spaceSetting => spaceSetting.Space, space => space.Settings, spaceSetting => spaceSetting.SpaceId, DeleteBehavior.Cascade);
        }
    }
}
