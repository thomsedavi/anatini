using Anatini.Server.Context.BuilderExtensions;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context
{
    public class AnatiniContext(ContextBase context)
    {
        public ContextBase Context => context;

        public async Task<int> Update(Entity entity)
        {
            context.Update(entity);

            return await context.SaveChangesAsync();
        }

        public async Task<int> Add(Entity entity)
        {
            context.Add(entity);

            return await context.SaveChangesAsync();
        }

        internal async Task<int> Remove(Entity entity)
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
        public DbSet<ChannelAlias> ChannelAliases { get; set; }
        public DbSet<ContentAlias> ContentAliases { get; set; }

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
            modelBuilder.Entity<ChannelAlias>().Configure();
            modelBuilder.Entity<ContentAlias>().Configure();
        }
    }

    public class User : BaseEntity
    {
        public required string Name { get; set; }
        public required string HashedPassword { get; set; }
        public required string DefaultSlug { get; set; }
        public required ICollection<UserOwnedEmail> Emails { get; set; }
        public ICollection<UserOwnedSession>? Sessions { get; set; }
        public required ICollection<UserOwnedAlias> Aliases { get; set; }
        public ICollection<UserOwnedChannel>? Channels { get; set; }
        public IList<string>? Permissions { get; set; }
    }

    [Owned]
    public class UserOwnedChannel : UserOwnedEntity
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string DefaultSlug { get; set; }
    }

    [Owned]
    public class UserOwnedEmail : UserOwnedEntity
    {
        public required string Address { get; set; }
        public required bool Verified { get; set; }
    }

    [Owned]
    public class UserOwnedAlias : UserOwnedEntity
    {
        public required string Slug { get; set; }
    }

    [Owned]
    public class UserOwnedSession : UserOwnedEntity
    {
        public required Guid Id { get; set; }
        public required string RefreshToken { get; set; }
        public required string IPAddress { get; set; }
        public required string UserAgent { get; set; }
        public required DateTime CreatedDateTimeUtc { get; set; }
        public required DateTime UpdatedDateTimeUtc { get; set; }
        public required bool Revoked { get; set; }
    }

    public class Channel : BaseEntity
    {
        public required string Name { get; set; }
        public required ICollection<ChannelOwnedUser> Users { get; set; }
        public required ICollection<ChannelOwnedAlias> Aliases { get; set; }
        public ICollection<ChannelOwnedContent>? TopDraftContents { get; set; }
        public ICollection<ChannelOwnedContent>? TopPublishedContents { get; set; }
        public required string DefaultSlug { get; set; }
    }

    [Owned]
    public class ChannelOwnedUser : ChannelOwnedEntity
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class ChannelOwnedAlias : ChannelOwnedEntity
    {
        public required string Slug { get; set; }
    }

    [Owned]
    public class ChannelOwnedContent : ChannelOwnedEntity
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string DefaultSlug { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
    }

    public class Content : BaseEntity
    {
        public required Guid ChannelId { get; set; }
        public required string Status { get; set; }
        public required string ContentType { get; set; }
        public required ICollection<ContentOwnedAlias> Aliases { get; set; }
        public required ContentOwnedVersion DraftVersion {  get; set; }
        public ContentOwnedVersion? PublishedVersion {  get; set; }
        public required string DefaultSlug { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
        public bool? Protected { get; set; }
    }

    [Owned]
    public class ContentOwnedVersion : ContentOwnedEntity
    {
        public required string Name { get; set; }
        public ICollection<ContentOwnedElement>? Elements { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class ContentOwnedAlias : ContentOwnedEntity
    {
        public required string Slug { get; set; }
    }

    [Owned]
    public class ContentOwnedElement
    {
        public required int Index { get; set; }
        public required string Tag { get; set; }
        public required Guid ContentOwnedVersionContentId { get; set; }
        public required Guid ContentOwnedVersionContentChannelId { get; set; }
        public string? Content { get; set; }
    }

    public class AttributeContent : ContentEntity
    {
        public required string Value { get; set; }
        public required string ContentSlug { get; set; }
        public required string ContentChannelSlug { get; set; }
        public required string ContentName { get; set; }
        public required string ChannelName { get; set; }
    }

    public class UserEvent : UserEntity
    {
        public required string EventType { get; set; }
        public required DateTime DateTimeUtc { get; set; }
        public required IDictionary<string, string> Data { get; set; }
    }

    public class UserEmail : UserEntity
    {
        public required string Address { get; set; }
        public string? VerificationCode { get; set; }
        public required bool Verified { get; set; }
    }

    public class UserToUserRelationship : UserEntity
    {
        public required Guid ToUserId { get; set; }
        public required string RelationshipType { get; set; }
    }

    public class UserAlias : AliasEntity
    {
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
    }

    public class ChannelAlias : AliasEntity
    {
        public required Guid ChannelId { get; set; }
        public required string ChannelName { get; set; }
    }

    public class ContentAlias : AliasEntity
    {
        public required Guid ContentChannelId { get; set; }
        public required Guid ContentId { get; set; }
        public required string ContentName { get; set; }
    }

    public abstract class Entity
    {
        public required string ItemId { get; set; }
        public string? ETag { get; set; }
        public int? Timestamp { get; set; }
    }

    public abstract class BaseEntity  : Entity
    {
        public required Guid Id { get; set; }
    }

    public abstract class UserEntity : Entity
    {
        public required Guid UserId { get; set; }
    }

    public abstract class ContentEntity : Entity
    {
        public required Guid ContentId { get; set; }
        public required Guid ContentChannelId { get; set; }
    }

    public abstract class AliasEntity : Entity
    {
        public required string Slug { get; set; }
    }

    public abstract class UserOwnedEntity
    {
        public required Guid UserId { get; set; }
    }

    public abstract class ChannelOwnedEntity
    {
        public required Guid ChannelId { get; set; }
    }

    public abstract class ContentOwnedEntity
    {
        public required Guid ContentChannelId { get; set; }
        public required Guid ContentId { get; set; }
    }
}
