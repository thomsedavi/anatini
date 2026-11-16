using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ContentBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Content> contentBuilder)
        {
            contentBuilder.ToTable("contents", tableBuilder => tableBuilder.HasCheckConstraint("ck_contents_user_id_xor_space_id", $"({contentBuilder.GetColumnName(nameof(Content.UserId))} IS NULL AND {contentBuilder.GetColumnName(nameof(Content.SpaceId))} IS NOT NULL) OR ({contentBuilder.GetColumnName(nameof(Content.SpaceId))} IS NULL AND {contentBuilder.GetColumnName(nameof(Content.UserId))} IS NOT NULL)"));

            contentBuilder.HasKey(content => content.Id);

            contentBuilder.Property(content => content.Id).Has(order: 0);
            contentBuilder.Property(content => content.ParentId).Has(order: 1);
            contentBuilder.Property(content => content.UserId).Has(order: 2);
            contentBuilder.Property(content => content.SpaceId).Has(order: 3);
            contentBuilder.Property(content => content.Type).Has(order: 4);
            contentBuilder.Property(content => content.Handle)!.Has(maxLength: 255, order: 5);
            contentBuilder.Property(content => content.Status).Has(order: 6);
            contentBuilder.Property(content => content.PublishedAtNz).Has(order: 7);
            contentBuilder.Property(content => content.Visibility).Has(order: 8);
            contentBuilder.Property(content => content.Title).Has(maxLength: 255, order: 9);
            contentBuilder.Property(content => content.Body)!.Has(order: 10);
            contentBuilder.Property(content => content.CurrentVersionNumber).Has(order: 11);
            contentBuilder.Property(content => content.ConcurrencyStamp)!.Has(order: 12).IsConcurrencyToken();
            contentBuilder.Property(content => content.CreatedAtUtc).Has(order: 13);
            contentBuilder.Property(content => content.UpdatedAtUtc).Has(order: 14);

            contentBuilder.HasOneWithMany(content => content.User, user => user.Contents, content => content.UserId, DeleteBehavior.Restrict, required: false);
            contentBuilder.HasOneWithMany(content => content.Space, space => space.Contents, content => content.SpaceId, DeleteBehavior.Restrict, required: false);
            contentBuilder.HasOneWithMany(content => content.Parent, content => content.Children, content => content.ParentId, DeleteBehavior.Cascade, required: false);

            var contentUserId = contentBuilder.GetColumnName(nameof(Content.UserId));
            var contentSpaceId = contentBuilder.GetColumnName(nameof(Content.SpaceId));
            var contentPublishedAtNz = contentBuilder.GetColumnName(nameof(Content.PublishedAtNz));
            var contentStatus = contentBuilder.GetColumnName(nameof(Content.Status));
            var contentType = contentBuilder.GetColumnName(nameof(Content.Type));

            var publishedStatus = (int)Status.Published;
            var eventType = (int)ContentType.Event;
            var eventOccurrenceType = (int)ContentType.EventOccurrence;
            var postType = (int)ContentType.Post;
            var workType = (int)ContentType.Work;

            contentBuilder.HasIndex(content => new { content.UserId, content.Handle }, "ix_events_user_id_handle").IsUnique().HasFilter($"{contentType} = {eventType} AND {contentUserId} IS NOT NULL").HasDatabaseName("ix_events_user_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Handle }, "ix_events_space_id_handle").IsUnique().HasFilter($"{contentType} = {eventType} AND {contentSpaceId} IS NOT NULL").HasDatabaseName("ix_events_space_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.ParentId, content.Handle }, "ix_event_occurrences_user_id_parent_id_handle").IsUnique().HasFilter($"{contentType} = {eventOccurrenceType} AND {contentUserId} IS NOT NULL").HasDatabaseName("ix_event_occurrences_user_id_parent_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.ParentId, content.Handle }, "ix_event_occurrences_space_id_parent_id_handle").IsUnique().HasFilter($"{contentType} = {eventOccurrenceType} AND {contentSpaceId} IS NOT NULL").HasDatabaseName("ix_event_occurrences_space_id_parent_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.Handle }, "ix_posts_user_id_handle").IsUnique().HasFilter($"{contentType} = {postType} AND {contentUserId} IS NOT NULL").HasDatabaseName("ix_posts_user_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Handle }, "ix_posts_space_id_handle").IsUnique().HasFilter($"{contentType} = {postType} AND {contentSpaceId} IS NOT NULL").HasDatabaseName("ix_posts_space_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.Handle }, "ix_works_user_id_handle").IsUnique().HasFilter($"{contentType} = {workType} AND {contentUserId} IS NOT NULL").HasDatabaseName("ix_works_user_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Handle }, "ix_works_space_id_handle").IsUnique().HasFilter($"{contentType} = {workType} AND {contentSpaceId} IS NOT NULL").HasDatabaseName("ix_works_space_id_handle");
            contentBuilder.HasIndex(content => content.PublishedAtNz).HasFilter($"{contentPublishedAtNz} IS NOT NULL AND {contentStatus} = {publishedStatus}").HasDatabaseName("ix_published_contents_date_nz");
            contentBuilder.HasIndex(content => content.Title).HasFilter($"{contentType} = {workType} AND {contentStatus} = {publishedStatus}").HasDatabaseName("ix_published_works_title");

            contentBuilder.HasGeneratedTsVectorColumn(content => content.SearchVector, "english", content => new { content.Title, content.Body }).HasIndex(content => content.SearchVector).HasMethod("GIN");
        }
    }
}
