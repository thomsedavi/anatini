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

            userBuilder.ToContainer("Users").HasPartitionKey(user => user.Guid);
            userBuilder.OwnsMany(user => user.Sessions, session => { session.WithOwner().HasForeignKey(session => session.UserGuid); session.HasKey(session => session.Guid); });
            userBuilder.OwnsMany(user => user.Emails, email => { email.WithOwner().HasForeignKey(email => email.UserGuid); email.HasKey(email => email.Guid); });
            userBuilder.OwnsMany(user => user.Invites, invite => { invite.WithOwner().HasForeignKey(invite => invite.UserGuid); invite.HasKey(invite => invite.Guid); });
            userBuilder.OwnsMany(user => user.Channels, channel => { channel.WithOwner().HasForeignKey(channel => channel.UserGuid); channel.HasKey(channel => channel.Guid); });
            userBuilder.OwnsMany(user => user.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => alias.UserGuid); alias.HasKey(alias => alias.Guid); });

            channelBuilder.ToContainer("Channels").HasPartitionKey(channel => channel.Guid);
            channelBuilder.OwnsMany(channel => channel.Users, user => { user.WithOwner().HasForeignKey(user => user.ChannelGuid); user.HasKey(user => user.Guid); });
            channelBuilder.OwnsMany(channel => channel.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => alias.ChannelGuid); alias.HasKey(alias => alias.Guid); });
            channelBuilder.OwnsMany(channel => channel.TopDraftPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelGuid); post.HasKey(post => post.Guid); });
            channelBuilder.OwnsMany(channel => channel.TopPublishedPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelGuid); post.HasKey(post => post.Guid); });

            postBuilder.ToContainer("Posts").HasPartitionKey(post => new { post.ChannelGuid, post.Guid });
            postBuilder.OwnsMany(post => post.Aliases, alias => { alias.WithOwner().HasForeignKey(alias => alias.PostGuid); alias.HasKey(alias => alias.Guid); });

            modelBuilder.Entity<UserEvent>().ToContainer("UserEvents").HasPartitionKey(userEvent => new { userEvent.UserGuid, userEvent.Type });
            modelBuilder.Entity<UserToUserRelationship>().ToContainer("UserToUserRelationships").HasPartitionKey(userToUserRelationship => new { userToUserRelationship.UserGuid, userToUserRelationship.Type, userToUserRelationship.ToUserGuid });

            modelBuilder.Entity<UserEmail>().ToContainer("UserEmails").HasPartitionKey(userEmail => userEmail.Address);
            modelBuilder.Entity<UserInvite>().ToContainer("UserInvites").HasPartitionKey(userInvite => userInvite.Code);

            modelBuilder.Entity<UserAlias>().ToContainer("UserAliases").HasPartitionKey(userAlias => userAlias.Slug);
            modelBuilder.Entity<ChannelAlias>().ToContainer("ChannelAliases").HasPartitionKey(channelAlias => channelAlias.Slug);
            modelBuilder.Entity<PostAlias>().ToContainer("PostAliases").HasPartitionKey(postAlias => new { postAlias.ChannelGuid, postAlias.Slug });
        }
    }

    public class User : Entity
    {
        public required string Name { get; set; }
        public required string HashedPassword { get; set; }
        public required ICollection<UserOwnedEmail> Emails { get; set; }
        public required ICollection<UserOwnedSession> Sessions { get; set; }
        public required ICollection<UserOwnedAlias> Aliases { get; set; }
        public ICollection<UserOwnedInvite>? Invites { get; set; }
        public ICollection<UserOwnedChannel>?  Channels { get; set; }
        public required Guid DefaultAliasGuid { get; set; }
    }

    [Owned]
    public class UserOwnedChannel : OwnedEntity
    {
        public required Guid UserGuid { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class UserOwnedEmail : OwnedEntity
    {
        public required Guid UserGuid { get; set; }
        public required string Address { get; set; }
        public required bool Verified { get; set; }
    }

    [Owned]
    public class UserOwnedAlias : OwnedEntity
    {
        public required Guid UserGuid { get; set; }
        public required string Slug { get; set; }
    }

    [Owned]
    public class UserOwnedInvite : OwnedEntity
    {
        public required Guid UserGuid { get; set; }
        public required string Code { get; set; }
        public required bool Used { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class UserOwnedSession : OwnedEntity
    {
        public required Guid UserGuid { get; set; }
        public required string RefreshToken { get; set; }
        public required string IPAddress { get; set; }
        public required string UserAgent { get; set; }
        public required DateTime CreatedDateUtc { get; set; }
        public required DateTime UpdatedDateUtc { get; set; }
        public required bool Revoked { get; set; }
    }

    public class Channel : Entity
    {
        public required string Name { get; set; }
        public required ICollection<ChannelOwnedUser> Users { get; set; }
        public required ICollection<ChannelOwnedAlias> Aliases { get; set; }
        public ICollection<ChannelOwnedPost>? TopDraftPosts { get; set; }
        public ICollection<ChannelOwnedPost>? TopPublishedPosts { get; set; }
        public required Guid DefaultAliasGuid { get; set; }
    }

    [Owned]
    public class ChannelOwnedUser : OwnedEntity
    {
        public required Guid ChannelGuid { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class ChannelOwnedAlias : OwnedEntity
    {
        public required Guid ChannelGuid { get; set; }
        public required string Slug { get; set; }
    }

    [Owned]
    public class ChannelOwnedPost : OwnedEntity
    {
        public required Guid ChannelGuid { get; set; }
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public required DateTime UpdatedDateUTC { get; set; }
    }

    public class Post : Entity
    {
        public required Guid ChannelGuid { get; set; }
        public required string Name { get; set; }
        public required DateOnly DateNZ { get; set; }
        public required ICollection<PostOwnedAlias> Aliases { get; set; }
        public required Guid DefaultAliasGuid { get; set; }
        public required DateTime UpdatedDateUTC { get; set; }

        // content, an array of objects with type, value, caption, platform (youtube) etc
        // tags
    }

    [Owned]
    public class PostOwnedAlias : OwnedEntity
    {
        public required Guid PostGuid { get; set; }
        public required string Slug { get; set; }
    }

    public class UserEvent : Entity
    {
        public required Guid UserGuid { get; set; }
        public required string Type { get; set; }
        public required DateTime DateUtc { get; set; }
        public required IDictionary<string, string> Data { get; set; }
    }

    public class UserEmail : Entity
    {
        public required string Address { get; set; }
        public required Guid UserGuid { get; set; }
        public string? VerificationCode { get; set; }
        public Guid? InvitedByUserGuid { get; set; }
        public Guid? InviteGuid { get; set; }
        public required bool Verified { get; set; }
    }

    public class UserToUserRelationship : Entity
    {
        public required Guid UserGuid { get; set; }
        public required Guid ToUserGuid { get; set; }
        public required string Type { get; set; }
    }

    public class UserInvite : Entity
    {
        public required string Code { get; set; }
        public required Guid NewUserGuid { get; set; }
        public required Guid InvitedByUserGuid { get; set; }
        public required DateOnly DateNZ { get; set; }
        public string? EmailAddress { get; set; }
    }

    public class UserAlias : Entity
    {
        public required string Slug { get; set; }
        public required Guid UserGuid { get; set; }
        public required string UserName { get; set; }
    }

    public class ChannelAlias : Entity
    {
        public required string Slug { get; set; }
        public required Guid ChannelGuid { get; set; }
        public required string ChannelName { get; set; }
    }

    public class PostAlias : Entity
    {
        public required string Slug { get; set; }
        public required Guid ChannelGuid { get; set; }
        public required Guid PostGuid { get; set; }
        public required string PostName { get; set; }
    }

    public abstract class Entity
    {
        public Guid SystemId { get; set; }
        public required Guid Guid { get; set; }
    }

    public abstract class OwnedEntity
    {
        public required Guid Guid { get; set; }
    }
}
