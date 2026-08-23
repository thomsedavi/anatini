using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class WorkBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Work> workBuilder)
        {
            workBuilder.ToTable("works", tableBuilder => tableBuilder.HasCheckConstraint("ck_works_user_id_xor_space_id", $"({workBuilder.GetColumnName(nameof(Work.UserId))} IS NULL AND {workBuilder.GetColumnName(nameof(Work.SpaceId))} IS NOT NULL) OR ({workBuilder.GetColumnName(nameof(Work.SpaceId))} IS NULL AND {workBuilder.GetColumnName(nameof(Work.UserId))} IS NOT NULL)"));

            workBuilder.HasKey(work => work.Id);

            workBuilder.Property(work => work.Id).Has(order: 0);
            workBuilder.Property(work => work.UserId).Has(order: 1);
            workBuilder.Property(work => work.SpaceId).Has(order: 2);
            workBuilder.Property(work => work.Handle)!.Has(maxLength: 255, order: 3);
            workBuilder.Property(work => work.Type).Has(order: 4);
            workBuilder.Property(work => work.Status).Has(order: 5);
            workBuilder.Property(work => work.PublishedAtNz).Has(order: 6);
            workBuilder.Property(work => work.Visibility).Has(order: 7);
            workBuilder.Property(work => work.Name)!.Has(maxLength: 255, order: 8);
            workBuilder.Property(work => work.Article).Has(order: 9);
            workBuilder.Property(work => work.Url)!.Has(maxLength: 2047, order: 10);
            workBuilder.Property(work => work.CurrentVersionNumber).Has(order: 11);
            workBuilder.Property(work => work.ConcurrencyStamp)!.Has(order: 12).IsConcurrencyToken();
            workBuilder.Property(work => work.CreatedAtUtc).Has(order: 13);
            workBuilder.Property(work => work.UpdatedAtUtc).Has(order: 14);

            workBuilder.HasOneWithMany(work => work.User, user => user.Works, work => work.UserId, DeleteBehavior.Restrict, required: false);
            workBuilder.HasOneWithMany(work => work.Space, space => space.Works, work => work.SpaceId, DeleteBehavior.Restrict, required: false);

            workBuilder.HasIndex(work => new { work.UserId, work.Type, work.Handle }).IsUnique().HasFilter($"{workBuilder.GetColumnName(nameof(Work.UserId))} IS NOT NULL");
            workBuilder.HasIndex(work => new { work.SpaceId, work.Type, work.Handle }).IsUnique().HasFilter($"{workBuilder.GetColumnName(nameof(Work.SpaceId))} IS NOT NULL");
            workBuilder.HasIndex(work => work.PublishedAtNz).HasFilter($"{workBuilder.GetColumnName(nameof(Work.PublishedAtNz))} IS NOT NULL AND {workBuilder.GetColumnName(nameof(Work.Status))} = {(int)Status.Published}").HasDatabaseName("ix_published_works_date_nz");
            workBuilder.HasIndex(work => work.Name).HasFilter($"{workBuilder.GetColumnName(nameof(Work.Status))} = {(int)Status.Published}").HasDatabaseName("ix_published_works_name");
        }
    }
}
