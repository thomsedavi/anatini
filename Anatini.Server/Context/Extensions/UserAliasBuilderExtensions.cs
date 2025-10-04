using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserAliasBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserAlias> userAliasBuilder)
        {
            userAliasBuilder.ToContainer("UserAliases");
            userAliasBuilder.HasKey(userAlias => userAlias.Id);
            userAliasBuilder.HasPartitionKey(userAlias => userAlias.Id);
            userAliasBuilder.Property(userAlias => userAlias.Id).ToJsonProperty("id");
            userAliasBuilder.Property(userAlias => userAlias.Slug).ToJsonProperty("slug");
            userAliasBuilder.Property(userAlias => userAlias.UserId).ToJsonProperty("userId");
            userAliasBuilder.Property(userAlias => userAlias.UserName).ToJsonProperty("userName");
        }
    }
}
