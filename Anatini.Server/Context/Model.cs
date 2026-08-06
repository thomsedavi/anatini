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
        public DbSet<ApplicationUserActivityEdge> UserActivityEdges { get; set; }
        public DbSet<ApplicationUserEventInstanceEdge> UserEventInstanceEdges { get; set; }

        public DbSet<Space> Spaces { get; set; }
        public DbSet<SpaceHandle> SpaceHandles { get; set; }
        public DbSet<SpaceImage> SpaceImages { get; set; }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityVersion> ActivityVersions { get; set; }
        public DbSet<ActivityImage> ActivityImages { get; set; }

        public DbSet<EventSeries> EventSeries { get; set; }
        public DbSet<EventException> EventExceptions { get; set; }
        public DbSet<EventInstance> EventInstances { get; set; }

        public IQueryable<Activity> Posts => Activities.Where(activity => activity.Type == ActivityType.Post);
        public IQueryable<Activity> Notes => Activities.Where(activity => activity.Type == ActivityType.Note);

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
            modelBuilder.Entity<ApplicationUserActivityEdge>().Configure();
            modelBuilder.Entity<ApplicationUserEventInstanceEdge>().Configure();

            modelBuilder.Entity<ApplicationRoleClaim>().Configure();
            modelBuilder.Entity<ApplicationRole>().Configure();

            modelBuilder.Entity<Space>().Configure();
            modelBuilder.Entity<SpaceHandle>().Configure();
            modelBuilder.Entity<SpaceImage>().Configure();

            modelBuilder.Entity<Activity>().Configure();
            modelBuilder.Entity<ActivityVersion>().Configure();
            modelBuilder.Entity<ActivityImage>().Configure();

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
