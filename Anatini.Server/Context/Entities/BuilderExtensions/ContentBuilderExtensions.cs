using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ContentBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Content> contentBuilder)
        {

            var parentId = contentBuilder.GetColumnName(nameof(Content.ParentId));
            var userId = contentBuilder.GetColumnName(nameof(Content.UserId));
            var spaceId = contentBuilder.GetColumnName(nameof(Content.SpaceId));
            var type = contentBuilder.GetColumnName(nameof(Content.Type));
            var status = contentBuilder.GetColumnName(nameof(Content.Status));
            var publishedAtUtc = contentBuilder.GetColumnName(nameof(Content.PublishedAtUtc));

            var publishedStatus = (int)Status.Published;
            var noteType = (int)ContentType.Note;
            var articleType = (int)ContentType.Article;
            var articleVersionType = (int)ContentType.ArticleVersion;
            var workType = (int)ContentType.Work;
            var workVersionType = (int)ContentType.WorkVersion;
            var eventType = (int)ContentType.Event;
            var eventOccurrenceType = (int)ContentType.EventOccurrence;

            contentBuilder.ToTable("contents", tableBuilder => {
                tableBuilder.HasCheckConstraint("ck_contents_check", $"({contentBuilder.GetColumnName(nameof(Content.UserId))} IS NULL AND {contentBuilder.GetColumnName(nameof(Content.SpaceId))} IS NOT NULL) OR ({contentBuilder.GetColumnName(nameof(Content.SpaceId))} IS NULL AND {contentBuilder.GetColumnName(nameof(Content.UserId))} IS NOT NULL)");
                tableBuilder.HasCheckConstraint("ck_contents_article_versions_check", $"{type} <> {articleVersionType} OR {parentId} IS NOT NULL");
                tableBuilder.HasCheckConstraint("ck_contents_work_versions_check", $"{type} <> {workVersionType} OR {parentId} IS NOT NULL");
                tableBuilder.HasCheckConstraint("ck_contents_event_occurrences_check", $"{type} <> {eventOccurrenceType} OR {parentId} IS NOT NULL");
            });

            contentBuilder.HasKey(content => content.Id);

            contentBuilder.Property(content => content.Id).Has(order: 0);
            contentBuilder.Property(content => content.ParentId).Has(order: 1);
            contentBuilder.Property(content => content.ActiveChildId).Has(order: 2);
            contentBuilder.Property(content => content.UserId).Has(order: 3);
            contentBuilder.Property(content => content.SpaceId).Has(order: 4);
            contentBuilder.Property(content => content.Type).Has(order: 5);
            contentBuilder.Property(content => content.Handle)!.Has(maxLength: 255, order: 6);
            contentBuilder.Property(content => content.Title).Has(maxLength: 255, order: 7);
            contentBuilder.Property(content => content.Body)!.Has(order: 8);
            contentBuilder.Property(content => content.Status).Has(order: 9);
            contentBuilder.Property(content => content.Visibility).Has(order: 10);
            contentBuilder.Property(content => content.PublishedAtUtc).Has(order: 11);
            contentBuilder.Property(content => content.TimeZoneId)!.Has(order: 12);
            contentBuilder.Property(content => content.Version).Has(order: 13);
            contentBuilder.Property(content => content.ConcurrencyStamp)!.Has(order: 14).IsConcurrencyToken();
            contentBuilder.Property(content => content.SearchVector)!.Has(order: 15);
            contentBuilder.Property(content => content.CreatedAtUtc).Has(order: 16);
            contentBuilder.Property(content => content.UpdatedAtUtc).Has(order: 17);

            contentBuilder.HasOneWithMany(content => content.User, user => user.Contents, content => content.UserId, DeleteBehavior.Restrict, required: false);
            contentBuilder.HasOneWithMany(content => content.Space, space => space.Contents, content => content.SpaceId, DeleteBehavior.Restrict, required: false);
            contentBuilder.HasOneWithMany(content => content.Parent, content => content.Children, content => content.ParentId, DeleteBehavior.Cascade, required: false);

            contentBuilder.HasIndex(content => new { content.UserId, content.Handle }, "ix_notes_user_id_handle").IsUnique().HasFilter($"{type} = {noteType} AND {userId} IS NOT NULL").HasDatabaseName("ix_notes_user_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Handle }, "ix_notes_space_id_handle").IsUnique().HasFilter($"{type} = {noteType} AND {spaceId} IS NOT NULL").HasDatabaseName("ix_notes_space_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.Handle }, "ix_articles_user_id_handle").IsUnique().HasFilter($"{type} = {articleType} AND {userId} IS NOT NULL").HasDatabaseName("ix_articles_user_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Handle }, "ix_articles_space_id_handle").IsUnique().HasFilter($"{type} = {articleType} AND {spaceId} IS NOT NULL").HasDatabaseName("ix_articles_space_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.ParentId, content.Handle }, "ix_article_versions_user_id_parent_id_handle").IsUnique().HasFilter($"{type} = {articleVersionType} AND {userId} IS NOT NULL").HasDatabaseName("ix_article_versions_user_id_parent_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.ParentId, content.Handle }, "ix_article_versions_space_id_parent_id_handle").IsUnique().HasFilter($"{type} = {articleVersionType} AND {spaceId} IS NOT NULL").HasDatabaseName("ix_article_versions_space_id_parent_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.Handle }, "ix_works_user_id_handle").IsUnique().HasFilter($"{type} = {workType} AND {userId} IS NOT NULL").HasDatabaseName("ix_works_user_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Handle }, "ix_works_space_id_handle").IsUnique().HasFilter($"{type} = {workType} AND {spaceId} IS NOT NULL").HasDatabaseName("ix_works_space_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.ParentId, content.Handle }, "ix_work_versions_user_id_parent_id_handle").IsUnique().HasFilter($"{type} = {workVersionType} AND {userId} IS NOT NULL").HasDatabaseName("ix_work_versions_user_id_parent_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.ParentId, content.Handle }, "ix_work_versions_space_id_parent_id_handle").IsUnique().HasFilter($"{type} = {workVersionType} AND {spaceId} IS NOT NULL").HasDatabaseName("ix_work_versions_space_id_parent_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.Handle }, "ix_events_user_id_handle").IsUnique().HasFilter($"{type} = {eventType} AND {userId} IS NOT NULL").HasDatabaseName("ix_events_user_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Handle }, "ix_events_space_id_handle").IsUnique().HasFilter($"{type} = {eventType} AND {spaceId} IS NOT NULL").HasDatabaseName("ix_events_space_id_handle");
            contentBuilder.HasIndex(content => new { content.UserId, content.ParentId, content.Handle }, "ix_event_occurrences_user_id_parent_id_handle").IsUnique().HasFilter($"{type} = {eventOccurrenceType} AND {userId} IS NOT NULL").HasDatabaseName("ix_event_occurrences_user_id_parent_id_handle");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.ParentId, content.Handle }, "ix_event_occurrences_space_id_parent_id_handle").IsUnique().HasFilter($"{type} = {eventOccurrenceType} AND {spaceId} IS NOT NULL").HasDatabaseName("ix_event_occurrences_space_id_parent_id_handle");
            contentBuilder.HasIndex(content => content.PublishedAtUtc).HasFilter($"{publishedAtUtc} IS NOT NULL AND {status} = {publishedStatus}").HasDatabaseName("ix_published_contents_date_utc");
            contentBuilder.HasIndex(content => content.Title).HasFilter($"{type} = {workType} AND {status} = {publishedStatus}").HasDatabaseName("ix_published_works_title");

            contentBuilder.HasGeneratedTsVectorColumn(content => content.SearchVector, "english", content => new { content.Title, content.Body }).HasIndex(content => content.SearchVector).HasMethod("GIN");
        }
    }
}
