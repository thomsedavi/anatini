using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserContentItemBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserContentRelationship> userContentBuilder)
        {
            userContentBuilder.ToTable("user_content_relationships");

            userContentBuilder.HasKey(userContentRelationship => new { userContentRelationship.SourceUserId, userContentRelationship.TargetContentId, userContentRelationship.Label });

            userContentBuilder.Property(userContentRelationship => userContentRelationship.SourceUserId).Has(order: 0);
            userContentBuilder.Property(userContentRelationship => userContentRelationship.TargetContentId).Has(order: 1);
            userContentBuilder.Property(userContentRelationship => userContentRelationship.Label).Has(order: 2);
            userContentBuilder.Property(userContentRelationship => userContentRelationship.CreatedAtUtc).Has(order: 3);

            userContentBuilder.HasOneWithMany(userContentRelationship => userContentRelationship.SourceUser, user => user.ContentRelationships, userContentRelationship => userContentRelationship.SourceUserId, DeleteBehavior.Restrict);
            userContentBuilder.HasOneWithMany(userContentRelationship => userContentRelationship.TargetContent, activity => activity.UserRelationships, userContentRelationship => userContentRelationship.TargetContentId, DeleteBehavior.Restrict);

            userContentBuilder.HasIndex(userContentRelationship => new { userContentRelationship.TargetContentId, userContentRelationship.Label, userContentRelationship.SourceUserId });
        }
    }
}
