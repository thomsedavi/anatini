using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserInviteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserInvite> userInviteBuilder)
        {
            userInviteBuilder.ToContainer("UserInvites");
            userInviteBuilder.HasKey(userInvite => userInvite.Code);
            userInviteBuilder.HasPartitionKey(userInvite => userInvite.Code);
            userInviteBuilder.Property(userInvite => userInvite.ItemId).ToJsonProperty("id");
            userInviteBuilder.Property(userInvite => userInvite.ETag).ToJsonProperty("_etag");
            userInviteBuilder.Property(userInvite => userInvite.Code).ToJsonProperty("Code");
        }
    }
}
