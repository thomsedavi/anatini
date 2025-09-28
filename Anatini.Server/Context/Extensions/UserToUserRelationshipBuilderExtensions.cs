using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserToUserRelationshipBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserToUserRelationship> userToUserRelationshipBuilder)
        {
            userToUserRelationshipBuilder.ToContainer("UserToUserRelationships").HasPartitionKey(userToUserRelationship => new { userToUserRelationship.UserId, userToUserRelationship.Type, userToUserRelationship.ToUserId });
            userToUserRelationshipBuilder.Property(userToUserRelationship => userToUserRelationship.Id).ToJsonProperty("id");
        }
    }
}
