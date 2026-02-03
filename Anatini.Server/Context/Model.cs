using Anatini.Server.Context.Entities;
using Anatini.Server.Context.Entities.BuilderExtensions;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context
{
    public class AnatiniContext(ContextBase context)
    {
        public ContextBase Context => context;

        public async Task<int> UpdateAsync(Entity entity)
        {
            context.Update(entity);

            return await context.SaveChangesAsync();
        }

        public async Task<int> AddAsync(Entity entity)
        {
            context.Add(entity);

            return await context.SaveChangesAsync();
        }

        internal async Task<int> RemoveAsync(Entity entity)
        {
            context.Remove(entity);

            return await context.SaveChangesAsync();
        }

        internal async Task<TEntity?> FindAsync<TEntity>(params object?[]? keyValues) where TEntity : Entity
        {
            return await context.FindAsync<TEntity>(keyValues);
        }
    }

    public class ContextBase : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserEmail> UserEmails { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<UserToUserRelationship> UserToUserRelationships { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<AttributeContent> AttributeContents { get; set; }
        public DbSet<UserAlias> UserAliases { get; set; }
        public DbSet<UserImage> UserImages { get; set; }
        public DbSet<ChannelAlias> ChannelAliases { get; set; }
        public DbSet<ChannelImage> ChannelImages { get; set; }
        public DbSet<ContentAlias> ContentAliases { get; set; }
        public DbSet<ContentImage> ContentImages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var keyVaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");

            // https://learn.microsoft.com/en-us/azure/key-vault/secrets/quick-create-net

            var accountEndpoint = "TODO";
            var accountKey = "TODO";
            var databaseName = "TODO";

            optionsBuilder.UseCosmos(accountEndpoint, accountKey, databaseName);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Configure();
            modelBuilder.Entity<Channel>().Configure();
            modelBuilder.Entity<Content>().Configure();
            modelBuilder.Entity<AttributeContent>().Configure();
            modelBuilder.Entity<UserEmail>().Configure();
            modelBuilder.Entity<UserEvent>().Configure();
            modelBuilder.Entity<UserToUserRelationship>().Configure();
            modelBuilder.Entity<UserAlias>().Configure();
            modelBuilder.Entity<UserImage>().Configure();
            modelBuilder.Entity<ChannelAlias>().Configure();
            modelBuilder.Entity<ChannelImage>().Configure();
            modelBuilder.Entity<ContentAlias>().Configure();
            modelBuilder.Entity<ContentImage>().Configure();
        }
    }

    public abstract class BaseEntity  : Entity
    {
        public required string Id { get; set; }
    }

    public abstract class AliasEntity : Entity
    {
        public required string Slug { get; set; }
    }

    public abstract class Entity
    {
        public required string ItemId { get; set; }
        public string? ETag { get; set; }
        public int? Timestamp { get; set; }
    }
}
