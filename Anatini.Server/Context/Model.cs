using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context
{
    public class AnatiniContext : DbContext
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
            var userBuilder = modelBuilder.Entity<User>();
            var channelBuilder = modelBuilder.Entity<Channel>();
            var postBuilder = modelBuilder.Entity<Post>();
            var userEmailBuilder = modelBuilder.Entity<UserEmail>();
            var userEventBuilder = modelBuilder.Entity<UserEvent>();
            var userToUserRelationshipBuilder = modelBuilder.Entity<UserToUserRelationship>();
            var userInviteBuilder = modelBuilder.Entity<UserInvite>();
            var userAliasBuilder = modelBuilder.Entity<UserAlias>();
            var channelAliasBuilder = modelBuilder.Entity<ChannelAlias>();
            var postAliasBuilder = modelBuilder.Entity<PostAlias>();

            userBuilder.ToContainer("Users").HasPartitionKey(user => user.Id);
            userBuilder.OwnsMany(user => user.Sessions, session => { session.WithOwner().HasForeignKey(session => session.UserId); session.HasKey(session => session.Id); });
            userBuilder.OwnsMany(user => user.Emails, email => { email.WithOwner().HasForeignKey(email => email.UserId); email.HasKey(email => email.Address); });
            userBuilder.OwnsMany(user => user.Invites, invite => { invite.WithOwner().HasForeignKey(invite => invite.UserId); invite.HasKey(invite => invite.Code); });
            userBuilder.OwnsMany(user => user.Channels, channel => { channel.WithOwner().HasForeignKey(channel => channel.UserId); channel.HasKey(channel => channel.Id); });
            userBuilder.OwnsMany(user => user.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => alias.UserId); alias.HasKey(alias => alias.Slug); });
            userBuilder.Property(user => user.Id).ToJsonProperty("id");

            channelBuilder.ToContainer("Channels").HasPartitionKey(channel => channel.Id);
            channelBuilder.OwnsMany(channel => channel.Users, user => { user.WithOwner().HasForeignKey(user => user.ChannelId); user.HasKey(user => user.Id); });
            channelBuilder.OwnsMany(channel => channel.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => alias.ChannelId); alias.HasKey(alias => alias.Slug); });
            channelBuilder.OwnsMany(channel => channel.TopDraftPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelId); post.HasKey(post => post.Id); });
            channelBuilder.OwnsMany(channel => channel.TopPublishedPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelId); post.HasKey(post => post.Id); });
            channelBuilder.Property(channel => channel.Id).ToJsonProperty("id");

            postBuilder.ToContainer("Posts").HasPartitionKey(post => new { post.ChannelId, post.Id });
            postBuilder.OwnsMany(post => post.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => new { alias.ChannelId, alias.PostId }); alias.HasKey(alias => alias.Slug); });
            postBuilder.Property(post => post.Id).ToJsonProperty("id");

            userEventBuilder.ToContainer("UserEvents").HasPartitionKey(userEvent => new { userEvent.UserId, userEvent.Type });
            userEventBuilder.Property(userEvent => userEvent.Id).ToJsonProperty("id");

            userToUserRelationshipBuilder.ToContainer("UserToUserRelationships").HasPartitionKey(userToUserRelationship => new { userToUserRelationship.UserId, userToUserRelationship.Type, userToUserRelationship.ToUserId });
            userToUserRelationshipBuilder.Property(userToUserRelationship => userToUserRelationship.Id).ToJsonProperty("id");

            userInviteBuilder.ToContainer("UserInvites");
            userInviteBuilder.HasKey(userInvite => userInvite.Code);
            userInviteBuilder.HasPartitionKey(userInvite => userInvite.Code);
            userInviteBuilder.Property(userInvite => userInvite.Code).ToJsonProperty("id");

            userEmailBuilder.ToContainer("UserEmails");
            userEmailBuilder.HasKey(emailAddress => emailAddress.Address);
            userEmailBuilder.HasPartitionKey(userEmail => userEmail.Address);
            userEmailBuilder.Property(userEmail => userEmail.Address).ToJsonProperty("id");

            userAliasBuilder.ToContainer("UserAliases");
            userAliasBuilder.HasKey(userAlias => userAlias.Slug);
            userAliasBuilder.HasPartitionKey(userAlias => userAlias.Slug);
            userAliasBuilder.Property(userAlias => userAlias.Slug).ToJsonProperty("id");

            channelAliasBuilder.ToContainer("ChannelAliases");
            channelAliasBuilder.HasKey(channelAlias => channelAlias.Slug);
            channelAliasBuilder.HasPartitionKey(channelAlias => channelAlias.Slug);
            channelAliasBuilder.Property(channelAlias => channelAlias.Slug).ToJsonProperty("id");

            postAliasBuilder.HasKey(postAlias => postAlias.Slug);
            postAliasBuilder.ToContainer("PostAliases");
            postAliasBuilder.HasPartitionKey(postAlias => new { postAlias.ChannelId, postAlias.Slug });
            postAliasBuilder.Property(postAlias => postAlias.Slug).ToJsonProperty("id");
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

    public class UserEvent : UserEntity
    {
        public required Guid Id { get; set; }
        public required string Type { get; set; }
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
        public required Guid Id { get; set; }
        public required Guid ToUserId { get; set; }
        public required string Type { get; set; }
    }

    public class UserInvite : Entity
    {
        public required string Code { get; set; }
        public required Guid NewUserId { get; set; }
        public required Guid InvitedByUserId { get; set; }
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
        public required Guid ChannelId { get; set; }
        public required Guid PostId { get; set; }
        public required string PostName { get; set; }
    }

    public abstract class Entity { }

    public abstract class BaseEntity : Entity
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
        public required Guid ChannelId { get; set; }
        public required Guid PostId { get; set; }
    }
}
