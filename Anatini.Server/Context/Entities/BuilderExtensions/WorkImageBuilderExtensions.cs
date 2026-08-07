using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class WorkImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<WorkImage> workImageBuilder)
        {
            workImageBuilder.ToTable("work_images");

            workImageBuilder.HasKey(workImage => new { workImage.WorkId, workImage.Handle });

            workImageBuilder.Property(workImage => workImage.WorkId).Has(order: 0);
            workImageBuilder.Property(workImage => workImage.Handle)!.Has(maxLength: 255, order: 1);
            workImageBuilder.Property(workImage => workImage.BlobName)!.Has(maxLength: 255, order: 2);
            workImageBuilder.Property(workImage => workImage.BlobContainerName)!.Has(maxLength: 255, order: 3);
            workImageBuilder.Property(workImage => workImage.AltText).Has(maxLength: 511, order: 4);
            workImageBuilder.Property(workImage => workImage.CreatedAtUtc).Has(order: 5);
            workImageBuilder.Property(workImage => workImage.UpdatedAtUtc).Has(order: 6);

            workImageBuilder.HasOneWithMany(workImage => workImage.Work, activity => activity.Images, workImage => workImage.WorkId, DeleteBehavior.Restrict);
        }
    }
}
