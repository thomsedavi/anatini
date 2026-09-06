using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserUserRelationshipBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserUserRelationship> userUserRelationshipBuilder)
        {
            userUserRelationshipBuilder.ToTable("user_user_relationships", tableBuilder => tableBuilder.HasCheckConstraint("ck_user_user_relationships_source_user_id_not_target_user_id", $"{userUserRelationshipBuilder.GetColumnName(nameof(ApplicationUserUserRelationship.SourceUserId))} <> {userUserRelationshipBuilder.GetColumnName(nameof(ApplicationUserUserRelationship.TargetUserId))}"));

            userUserRelationshipBuilder.HasKey(userUserRelationship => new { userUserRelationship.SourceUserId, userUserRelationship.TargetUserId, userUserRelationship.Label });

            userUserRelationshipBuilder.Property(userUserRelationship => userUserRelationship.SourceUserId).Has(order: 0);
            userUserRelationshipBuilder.Property(userUserRelationship => userUserRelationship.TargetUserId).Has(order: 1);
            userUserRelationshipBuilder.Property(userUserRelationship => userUserRelationship.Label).Has(order: 2);
            userUserRelationshipBuilder.Property(userUserRelationship => userUserRelationship.CreatedAtUtc).Has(order: 3);

            userUserRelationshipBuilder.HasOneWithMany(userUserRelationship => userUserRelationship.SourceUser, user => user.GivenUserRelationships, userUserRelationship => userUserRelationship.SourceUserId, DeleteBehavior.Restrict);
            userUserRelationshipBuilder.HasOneWithMany(userUserRelationship => userUserRelationship.TargetUser, user => user.ReceivedUserRelationships, userUserRelationship => userUserRelationship.TargetUserId, DeleteBehavior.Restrict);

            userUserRelationshipBuilder.HasIndex(userUserRelationship => new { userUserRelationship.TargetUserId, userUserRelationship.Label, userUserRelationship.SourceUserId });
        }
    }
}
