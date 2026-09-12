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
            contentBuilder.Property(content => content.UserId).Has(order: 1);
            contentBuilder.Property(content => content.SpaceId).Has(order: 2);
            contentBuilder.Property(content => content.Type).Has(order: 3);
            contentBuilder.Property(content => content.Handle)!.Has(maxLength: 255, order: 4);
            contentBuilder.Property(content => content.Status).Has(order: 5);
            contentBuilder.Property(content => content.PublishedAtNz).Has(order: 6);
            contentBuilder.Property(content => content.Visibility).Has(order: 7);
            contentBuilder.Property(content => content.Name).Has(maxLength: 255, order: 8);
            contentBuilder.Property(content => content.Article)!.Has(order: 9);
            contentBuilder.Property(content => content.CurrentVersionNumber).Has(order: 10);
            contentBuilder.Property(content => content.ConcurrencyStamp)!.Has(order: 11).IsConcurrencyToken();
            contentBuilder.Property(content => content.CreatedAtUtc).Has(order: 12);
            contentBuilder.Property(content => content.UpdatedAtUtc).Has(order: 13);

            contentBuilder.HasOneWithMany(content => content.User, user => user.Contents, content => content.UserId, DeleteBehavior.Restrict, required: false);
            contentBuilder.HasOneWithMany(content => content.Space, space => space.Contents, content => content.SpaceId, DeleteBehavior.Restrict, required: false);

            var contentUserId = contentBuilder.GetColumnName(nameof(Content.UserId));
            var contentSpaceId = contentBuilder.GetColumnName(nameof(Content.SpaceId));
            var contentPublishedAtNz = contentBuilder.GetColumnName(nameof(Content.PublishedAtNz));
            var contentStatus = contentBuilder.GetColumnName(nameof(Content.Status));
            var contentType = contentBuilder.GetColumnName(nameof(Content.Type));

            var publishedStatus = (int)Status.Published;
            var workType = (int)ContentType.Work;

            contentBuilder.HasIndex(content => new { content.UserId, content.Type, content.Handle }).IsUnique().HasFilter($"{contentUserId} IS NOT NULL");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Type, content.Handle }).IsUnique().HasFilter($"{contentSpaceId} IS NOT NULL");
            contentBuilder.HasIndex(content => content.PublishedAtNz).HasFilter($"{contentPublishedAtNz} IS NOT NULL AND {contentStatus} = {publishedStatus}").HasDatabaseName("ix_published_contents_date_nz");
            contentBuilder.HasIndex(content => content.Name).HasFilter($"{contentType} = {workType} AND {contentStatus} = {publishedStatus}").HasDatabaseName("ix_published_works_name");

            contentBuilder.HasGeneratedTsVectorColumn(content => content.SearchVector, "english", content => new { content.Name, content.Article }).HasIndex(content => content.SearchVector).HasMethod("GIN");
        }
    }
}
