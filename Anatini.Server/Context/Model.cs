using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.BuilderExtensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anatini.Server.Context
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>(options)
    {
        public DbSet<ApplicationUserEmail> UserEmails { get; set; }
        public DbSet<ApplicationUserHandle> UserHandles { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<ApplicationUserImage> UserImages { get; set; }
        public DbSet<ApplicationUserTrust> UserTrusts { get; set; }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelHandle> ChannelHandles { get; set; }
        public DbSet<ChannelImage> ChannelImages { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostVersion> PostVersions { get; set; }
        public DbSet<PostHandle> PostHandles { get; set; }
        public DbSet<PostImage> PostImages { get; set; }

        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteImage> NoteImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().Configure();
            modelBuilder.Entity<ApplicationUserEmail>().Configure();
            modelBuilder.Entity<ApplicationUserHandle>().Configure();
            modelBuilder.Entity<ApplicationUserImage>().Configure();
            modelBuilder.Entity<ApplicationUserClaim>().Configure();
            modelBuilder.Entity<ApplicationUserLogin>().Configure();
            modelBuilder.Entity<ApplicationUserToken>().Configure();
            modelBuilder.Entity<ApplicationUserRole>().Configure();
            modelBuilder.Entity<ApplicationUserTrust>().Configure();
            modelBuilder.Entity<ApplicationUserChannel>().Configure();

            modelBuilder.Entity<ApplicationRoleClaim>().Configure();
            modelBuilder.Entity<ApplicationRole>().Configure();

            modelBuilder.Entity<Channel>().Configure();
            modelBuilder.Entity<ChannelHandle>().Configure();
            modelBuilder.Entity<ChannelImage>().Configure();

            modelBuilder.Entity<Post>().Configure();
            modelBuilder.Entity<PostVersion>().Configure();
            modelBuilder.Entity<PostImage>().Configure();
            modelBuilder.Entity<PostHandle>().Configure();

            modelBuilder.Entity<Note>().Configure();
            modelBuilder.Entity<NoteImage>().Configure();

            modelBuilder.Entity<Log>().Configure();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;

            //var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                // TODO do I want to do anything similar with the new Postgres database?
                //entry.Property("").CurrentValue = Guid.NewGuid();
                //entry.Property("ETag").CurrentValue = Guid.NewGuid();
                //entry.Property("UpdatedOn").CurrentValue = utcNow;

                //
                //if (entry.State == EntityState.Added)
                //{
                //    entry.Property("CreatedOn").CurrentValue = now;
                //}
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }

    public static class EntityTypeBuilderExtensions
    {
        public static string GetColumnName(this EntityTypeBuilder entityTypeBuilder, string name)
        {
            return entityTypeBuilder.Metadata.FindProperty(name)!.GetColumnName();
        }
    }
}
