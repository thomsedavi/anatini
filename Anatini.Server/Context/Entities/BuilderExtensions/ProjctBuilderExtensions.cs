using Anatini.Server.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ProjctBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<Project> projectBuilder)
        {
            projectBuilder.ToTable("projects", tableBuilder => tableBuilder.HasCheckConstraint("ck_projects_user_id_xor_space_id", $"({projectBuilder.GetColumnName(nameof(Project.UserId))} IS NULL AND {projectBuilder.GetColumnName(nameof(Project.SpaceId))} IS NOT NULL) OR ({projectBuilder.GetColumnName(nameof(Project.SpaceId))} IS NULL AND {projectBuilder.GetColumnName(nameof(Project.UserId))} IS NOT NULL)"));

            projectBuilder.Property(project => project.Id).Has(order: 0);
            projectBuilder.Property(project => project.UserId).Has(order: 1);
            projectBuilder.Property(project => project.SpaceId).Has(order: 2);
            projectBuilder.Property(project => project.Handle)!.Has(maxLength: 255, order: 3);
            projectBuilder.Property(project => project.Status).Has(order: 4);
            projectBuilder.Property(project => project.PublishedAtNz).Has(order: 5);
            projectBuilder.Property(project => project.Visibility).Has(order: 6);
            projectBuilder.Property(project => project.Name)!.Has(maxLength: 255, order: 7);
            projectBuilder.Property(project => project.CreatedAtUtc).Has(order: 8);
            projectBuilder.Property(project => project.UpdatedAtUtc).Has(order: 9);

            projectBuilder.HasOneWithMany(project => project.User, user => user.Projects, project => project.UserId, DeleteBehavior.Restrict, required: false);
            projectBuilder.HasOneWithMany(project => project.Space, space => space.Projects, project => project.SpaceId, DeleteBehavior.Restrict, required: false);

            projectBuilder.HasIndex(project => new { project.UserId, project.Type, project.Handle }).IsUnique().HasFilter($"{projectBuilder.GetColumnName(nameof(Project.UserId))} IS NOT NULL");
            projectBuilder.HasIndex(project => new { project.SpaceId, project.Type, project.Handle }).IsUnique().HasFilter($"{projectBuilder.GetColumnName(nameof(Project.SpaceId))} IS NOT NULL");
            projectBuilder.HasIndex(project => project.PublishedAtNz).HasFilter($"{projectBuilder.GetColumnName(nameof(Project.PublishedAtNz))} IS NOT NULL AND {projectBuilder.GetColumnName(nameof(Project.Status))} = {(int)Status.Published}").HasDatabaseName("ix_published_projects_date_nz");
        }
    }
}
