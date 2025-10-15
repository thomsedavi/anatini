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
        public DbSet<UserInvite> UserInvites { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<UserToUserRelationship> UserToUserRelationships { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<UserAlias> UserAliases { get; set; }
        public DbSet<ChannelAlias> ChannelAliases { get; set; }
        public DbSet<PostAlias> PostAliases { get; set; }

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
            modelBuilder.Entity<Post>().Configure();
            modelBuilder.Entity<UserEmail>().Configure();
            modelBuilder.Entity<UserEvent>().Configure();
            modelBuilder.Entity<UserToUserRelationship>().Configure();
            modelBuilder.Entity<UserInvite>().Configure();
            modelBuilder.Entity<UserAlias>().Configure();
            modelBuilder.Entity<ChannelAlias>().Configure();
            modelBuilder.Entity<PostAlias>().Configure();
        }
    }

    public class User : BaseEntity
    {
        public required string Name { get; set; }
        public required string HashedPassword { get; set; }
        public required ICollection<UserOwnedEmail> Emails { get; set; }
        public required ICollection<UserOwnedSession> Sessions { get; set; }
        public required ICollection<UserOwnedAlias> Aliases { get; set; }
        public ICollection<UserOwnedInvite>? Invites { get; set; }
        public ICollection<UserOwnedChannel>? Channels { get; set; }
        public required string DefaultSlug { get; set; }
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
    public class UserOwnedInvite : UserOwnedEntity
    {
        public required string Code { get; set; }
        public required bool Used { get; set; }
        public required DateOnly DateOnlyNZ { get; set; }
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
        public ICollection<ChannelOwnedPost>? TopDraftPosts { get; set; }
        public ICollection<ChannelOwnedPost>? TopPublishedPosts { get; set; }
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
    public class ChannelOwnedPost : ChannelOwnedEntity
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string DefaultSlug { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
    }

    public class Post : BaseEntity
    {
        public required Guid ChannelId { get; set; }
        public required string Name { get; set; }
        public required DateOnly DateOnlyNZ { get; set; }
        public required ICollection<PostOwnedAlias> Aliases { get; set; }
        public IList<PostOwnedElement>? Elements { get; set; }
        public required string DefaultSlug { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }

        // content, an array of objects with type, value, caption, platform (youtube) etc
        // tags
    }

    [Owned]
    public class PostOwnedAlias : PostOwnedEntity
    {
        public required string Slug { get; set; }
    }

    [Owned]
    public class PostOwnedElement : PostOwnedEntity
    {
        public required string Tag { get; set; }
        public string? Content { get; set; }
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
        public Guid? InvitedByUserId { get; set; }
        public string? InviteCode { get; set; }
        public required bool Verified { get; set; }
    }

    public class UserToUserRelationship : UserEntity
    {
        public required Guid ToUserId { get; set; }
        public required string RelationshipType { get; set; }
    }

    public class UserInvite : UserEntity
    {
        public required string Code { get; set; }
        public required Guid NewUserId { get; set; }
        public required DateOnly DateOnlyNZ { get; set; }
        public string? EmailAddress { get; set; }
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

    public class PostAlias : AliasEntity
    {
        public required Guid PostChannelId { get; set; }
        public required Guid PostId { get; set; }
        public required string PostName { get; set; }
    }

    public abstract class Entity
    {
        public required string ItemId { get; set; }
    }

    public abstract class BaseEntity  : Entity
    {
        public required Guid Id { get; set; }
    }

    public abstract class UserEntity : Entity
    {
        public required Guid UserId { get; set; }
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

    public abstract class PostOwnedEntity
    {
        public required Guid PostChannelId { get; set; }
        public required Guid PostId { get; set; }
    }
}
