using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserAlias> userAliasBuilder)
        {
            userAliasBuilder.ToContainer("UserAliases");
            userAliasBuilder.HasKey(userAlias => userAlias.Slug);
            userAliasBuilder.HasPartitionKey(userAlias => userAlias.Slug);
            userAliasBuilder.Property(userAlias => userAlias.ItemId).ToJsonProperty("id");
            userAliasBuilder.Property(userAlias => userAlias.Slug).ToJsonProperty("Slug");
        }
    }
}
