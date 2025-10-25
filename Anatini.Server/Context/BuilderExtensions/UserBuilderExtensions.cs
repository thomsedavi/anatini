using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<User> userBuilder)
        {
            userBuilder.ToContainer("Users");
            userBuilder.HasKey(user => user.Id);
            userBuilder.HasPartitionKey(user => user.Id);
            userBuilder.Property(user => user.ItemId).ToJsonProperty("id");
            userBuilder.Property(user => user.Id).ToJsonProperty("Id");
            userBuilder.Property(user => user.ETag).ToJsonProperty("_etag");
            userBuilder.OwnsMany(user => user.Aliases, aliasBuildAction => { aliasBuildAction.HasKey(userOwnedAlias => userOwnedAlias.Slug); });
            userBuilder.OwnsMany(user => user.Emails, emailBuildAction => { emailBuildAction.HasKey(userOwnedEmail => userOwnedEmail.Address); });
            userBuilder.OwnsMany(user => user.Channels, channelBuildAction => { channelBuildAction.HasKey(userOwnedChannel => userOwnedChannel.Id); });
            userBuilder.OwnsMany(user => user.Sessions, sessionsBuildAction => { sessionsBuildAction.HasKey(userOwnedSession => userOwnedSession.Id); });
        }
    }
}
