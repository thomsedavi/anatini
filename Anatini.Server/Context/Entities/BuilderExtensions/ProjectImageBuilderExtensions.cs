using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class ProjectImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ProjectImage> projectImageBuilder)
        {
            projectImageBuilder.ToTable("project_images");

            projectImageBuilder.HasKey(projectImage => new { projectImage.ProjectId, projectImage.Handle });

            projectImageBuilder.Property(projectImage => projectImage.ProjectId).Has(order: 0);
            projectImageBuilder.Property(projectImage => projectImage.Handle)!.Has(maxLength: 255, order: 1);
            projectImageBuilder.Property(projectImage => projectImage.BlobName)!.Has(maxLength: 255, order: 2);
            projectImageBuilder.Property(projectImage => projectImage.BlobContainerName)!.Has(maxLength: 255, order: 3);
            projectImageBuilder.Property(projectImage => projectImage.AltText).Has(maxLength: 511, order: 4);
            projectImageBuilder.Property(projectImage => projectImage.CreatedAtUtc).Has(order: 5);
            projectImageBuilder.Property(projectImage => projectImage.UpdatedAtUtc).Has(order: 6);

            projectImageBuilder.HasOneWithMany(projectImage => projectImage.Project, activity => activity.Images, projectImage => projectImage.ProjectId, DeleteBehavior.Restrict);
        }
    }
}
