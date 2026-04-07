using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context.Entities.BuilderExtensions
{
    public static class UserImageBuilderExtensions
    {
        public static void Configure(this EntityTypeBuilder<ApplicationUserImage> userImageBuilder)
        {
            userImageBuilder.ToTable("user_images");

            userImageBuilder.HasKey(userImage => userImage.Id);

            userImageBuilder.Property(userImage => userImage.AltText).HasMaxLength(256);
            userImageBuilder.Property(userImage => userImage.CreatedAtUtc).HasColumnType("timestamp with time zone");
            userImageBuilder.Property(userImage => userImage.UpdatedAtUtc).HasColumnType("timestamp with time zone");
            userImageBuilder.Property(userImage => userImage.BlobName).HasMaxLength(64);
            userImageBuilder.Property(userImage => userImage.BlobContainerName).HasMaxLength(16);

            userImageBuilder.HasOne(userImage => userImage.User).WithMany(user => user.Images).HasForeignKey(userImage => userImage.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
