using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Extensions
{
    public static class UserToUserRelationshipBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserToUserRelationship> userToUserRelationshipBuilder)
        {
            userToUserRelationshipBuilder.ToContainer("UserToUserRelationships");
            userToUserRelationshipBuilder.HasPartitionKey(userToUserRelationship => new { userToUserRelationship.UserId, userToUserRelationship.RelationshipType, userToUserRelationship.ToUserId });
            userToUserRelationshipBuilder.Property(userToUserRelationship => userToUserRelationship.Id).ToJsonProperty("id");
            userToUserRelationshipBuilder.Property(userToUserRelationship => userToUserRelationship.UserId).ToJsonProperty("userId");
            userToUserRelationshipBuilder.Property(userToUserRelationship => userToUserRelationship.RelationshipType).ToJsonProperty("relationshipType");
            userToUserRelationshipBuilder.Property(userToUserRelationship => userToUserRelationship.ToUserId).ToJsonProperty("toUserId");
        }
    }
}
