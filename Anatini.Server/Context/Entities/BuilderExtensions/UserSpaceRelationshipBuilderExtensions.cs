using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserSpaceRelationshipBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserSpaceRelationship> userSpaceRelationshipBuilder)
        {
            userSpaceRelationshipBuilder.ToTable("user_space_relationships");

            userSpaceRelationshipBuilder.HasKey(userSpaceRelationship => new { userSpaceRelationship.SourceUserId, userSpaceRelationship.TargetSpaceId, userSpaceRelationship.Label });

            userSpaceRelationshipBuilder.Property(userSpaceRelationship => userSpaceRelationship.SourceUserId).Has(order: 0);
            userSpaceRelationshipBuilder.Property(userSpaceRelationship => userSpaceRelationship.TargetSpaceId).Has(order: 1);
            userSpaceRelationshipBuilder.Property(userSpaceRelationship => userSpaceRelationship.Label).Has(order: 2);
            userSpaceRelationshipBuilder.Property(userSpaceRelationship => userSpaceRelationship.CreatedAtUtc).Has(order: 3);

            userSpaceRelationshipBuilder.HasOneWithMany(userSpaceRelationship => userSpaceRelationship.SourceUser, user => user.SpaceRelationships, userSpaceRelationship => userSpaceRelationship.SourceUserId, DeleteBehavior.Restrict);
            userSpaceRelationshipBuilder.HasOneWithMany(userSpaceRelationship => userSpaceRelationship.TargetSpace, space => space.UserRelationships, userSpaceRelationship => userSpaceRelationship.TargetSpaceId, DeleteBehavior.Restrict);

            userSpaceRelationshipBuilder.HasIndex(userSpaceRelationship => new { userSpaceRelationship.TargetSpaceId, userSpaceRelationship.Label, userSpaceRelationship.SourceUserId });
        }
    }
}
