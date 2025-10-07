using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.BuilderExtensions
{
    public static class UserToUserRelationshipBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<UserToUserRelationship> userToUserRelationshipBuilder)
        {
            userToUserRelationshipBuilder.ToContainer("UserToUserRelationships");
            userToUserRelationshipBuilder.HasKey(userToUserRelationship => new { userToUserRelationship.UserId, userToUserRelationship.RelationshipType, userToUserRelationship.ToUserId });
            userToUserRelationshipBuilder.HasPartitionKey(userToUserRelationship => new { userToUserRelationship.UserId, userToUserRelationship.RelationshipType, userToUserRelationship.ToUserId });
            userToUserRelationshipBuilder.Property(userToUserRelationship => userToUserRelationship.ItemId).ToJsonProperty("id");
        }
    }
}
