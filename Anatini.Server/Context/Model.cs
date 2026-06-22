using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.BuilderExtensions;
using Anatini.Server.Enums;
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
        public DbSet<ApplicationUserUserEdge> UserUserEdges { get; set; }
        public DbSet<ApplicationUserSpaceEdge> UserSpaceEdges { get; set; }
        public DbSet<ApplicationUserContentEdge> UserContentEdges { get; set; }

        public DbSet<Space> Spaces { get; set; }
        public DbSet<SpaceHandle> SpaceHandles { get; set; }
        public DbSet<SpaceImage> SpaceImages { get; set; }

        public DbSet<Content> Contents { get; set; }
        public DbSet<ContentVersion> ContentVersions { get; set; }
        public DbSet<ContentImage> ContentImages { get; set; }

        public DbSet<EventSeries> EventSeries { get; set; }
        public DbSet<EventException> EventExceptions { get; set; }
        public DbSet<EventInstance> EventInstances { get; set; }

        public IQueryable<Content> Posts => Contents.Where(content => content.Type == ContentType.Post);
        public IQueryable<Content> Notes => Contents.Where(content => content.Type == ContentType.Note);

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
            modelBuilder.Entity<ApplicationUserUserEdge>().Configure();
            modelBuilder.Entity<ApplicationUserSpaceEdge>().Configure();
            modelBuilder.Entity<ApplicationUserContentEdge>().Configure();

            modelBuilder.Entity<ApplicationRoleClaim>().Configure();
            modelBuilder.Entity<ApplicationRole>().Configure();

            modelBuilder.Entity<Space>().Configure();
            modelBuilder.Entity<SpaceHandle>().Configure();
            modelBuilder.Entity<SpaceImage>().Configure();

            modelBuilder.Entity<Content>().Configure();
            modelBuilder.Entity<ContentVersion>().Configure();
            modelBuilder.Entity<ContentImage>().Configure();

            modelBuilder.Entity<EventSeries>().Configure();
            modelBuilder.Entity<EventException>().Configure();
            modelBuilder.Entity<EventInstance>().Configure();

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
