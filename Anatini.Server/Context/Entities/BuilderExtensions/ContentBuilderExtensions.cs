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
            contentBuilder.Property(content => content.Handle)!.Has(maxLength: 256, order: 3);
            contentBuilder.Property(content => content.Type).Has(order: 4);
            contentBuilder.Property(content => content.Status).Has(order: 5);
            contentBuilder.Property(content => content.PublishedAtUtc).Has(order: 6);
            contentBuilder.Property(content => content.Visibility).Has(order: 7);
            contentBuilder.Property(content => content.Name).Has(maxLength: 256, order: 8);
            contentBuilder.Property(content => content.Article).Has(order: 9);
            contentBuilder.Property(content => content.Url).Has(maxLength: 2048, order: 10);
            contentBuilder.Property(content => content.CurrentVersionNumber).Has(order: 11);
            contentBuilder.Property(content => content.ConcurrencyStamp)!.Has(order: 12).IsConcurrencyToken();
            contentBuilder.Property(content => content.CreatedAtUtc).Has(order: 13);
            contentBuilder.Property(content => content.UpdatedAtUtc).Has(order: 14);

            contentBuilder.HasOneWithMany(content => content.User, user => user.Contents, content => content.UserId, DeleteBehavior.Restrict, required: false);
            contentBuilder.HasOneWithMany(content => content.Space, space => space.Contents, content => content.SpaceId, DeleteBehavior.Restrict, required: false);

            contentBuilder.HasIndex(content => new { content.UserId, content.Type, content.Handle }).IsUnique().HasFilter($"{contentBuilder.GetColumnName(nameof(Content.UserId))} IS NOT NULL");
            contentBuilder.HasIndex(content => new { content.SpaceId, content.Type, content.Handle }).IsUnique().HasFilter($"{contentBuilder.GetColumnName(nameof(Content.SpaceId))} IS NOT NULL");
            contentBuilder.HasIndex(content => content.PublishedAtUtc ).HasFilter($"{contentBuilder.GetColumnName(nameof(Content.Status))} = {(int)Status.Published}").HasDatabaseName("ix_published_contents_date_nz");
        }
    }
}
