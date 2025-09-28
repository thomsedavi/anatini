using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserInviteBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserInvite> userInviteBuilder)
        {
            userInviteBuilder.ToContainer("UserInvites").HasPartitionKey(userInvite => userInvite.Code);
        }
    }
}
