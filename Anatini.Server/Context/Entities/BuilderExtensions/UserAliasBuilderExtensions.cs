using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserAlias> userAliasBuilder)
        {
            userAliasBuilder.ToContainer("UserAliases");
            userAliasBuilder.HasKey(userAlias => userAlias.Slug);
            userAliasBuilder.HasPartitionKey(userAlias => userAlias.Slug);
            userAliasBuilder.Property(userAlias => userAlias.ItemId).ToJsonProperty("id");
            userAliasBuilder.Property(userAlias => userAlias.ETag).ToJsonProperty("_etag");
            userAliasBuilder.Property(userAlias => userAlias.Timestamp).ToJsonProperty("_ts");
            userAliasBuilder.Property(userAlias => userAlias.Slug).ToJsonProperty("Slug");
        }
    }
}
