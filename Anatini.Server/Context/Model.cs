using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context
{
    public class AnatiniContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<UserSlug> UserSlugs { get; set; }
        public DbSet<ChannelSlug> ChannelSlugs { get; set; }
        public DbSet<PostSlug> PostSlugs { get; set; }

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

            userBuilder.ToContainer("Users").HasPartitionKey(user => user.Id);
            userBuilder.OwnsMany(user => user.Sessions, session => { session.WithOwner().HasForeignKey(session => session.UserId); session.HasKey(session => session.Id); });
            userBuilder.OwnsMany(user => user.Emails, email => { email.WithOwner().HasForeignKey(email => email.UserId); email.HasKey(email => email.Id); });
            userBuilder.OwnsMany(user => user.Invites, invite => { invite.WithOwner().HasForeignKey(invite => invite.UserId); invite.HasKey(invite => invite.Id); });
            userBuilder.OwnsMany(user => user.Channels, channel => { channel.WithOwner().HasForeignKey(channel => channel.UserId); channel.HasKey(channel => channel.Id); });
            userBuilder.OwnsMany(user => user.Slugs, slug => { slug.WithOwner().HasForeignKey(slug => slug.UserId); slug.HasKey(slug => slug.Id); });

            channelBuilder.ToContainer("Channels").HasPartitionKey(channel => channel.Id);
            channelBuilder.OwnsMany(channel => channel.Users, user => { user.WithOwner().HasForeignKey(user => user.ChannelId); user.HasKey(user => user.Id); });
            channelBuilder.OwnsMany(channel => channel.Slugs, slug => { slug.WithOwner().HasForeignKey(slug => slug.ChannelId); slug.HasKey(slug => slug.Id); });
            channelBuilder.OwnsMany(channel => channel.TopDraftPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelId); post.HasKey(post => post.Id); });
            channelBuilder.OwnsMany(channel => channel.TopPublishedPosts, post => { post.WithOwner().HasForeignKey(post => post.ChannelId); post.HasKey(post => post.Id); });

            postBuilder.ToContainer("Posts").HasPartitionKey(post => post.Id);
            postBuilder.OwnsMany(post => post.Slugs, slug => { slug.WithOwner().HasForeignKey(slug => slug.PostId); slug.HasKey(slug => slug.Id); });

            modelBuilder.Entity<Event>().ToContainer("Events").HasPartitionKey(@event => new { @event.UserId, @event.Type });
            modelBuilder.Entity<Relationship>().ToContainer("Relationships").HasPartitionKey(relationship => new { relationship.UserId, relationship.Type, relationship.ToUserId });

            modelBuilder.Entity<Email>().ToContainer("Emails").HasPartitionKey(email => email.Address);
            modelBuilder.Entity<Invite>().ToContainer("Invites").HasPartitionKey(invite => invite.Code);

            modelBuilder.Entity<UserSlug>().ToContainer("UserSlugs").HasPartitionKey(userSlug => userSlug.Slug);
            modelBuilder.Entity<ChannelSlug>().ToContainer("ChannelSlugs").HasPartitionKey(channelSlug => channelSlug.Slug);
            modelBuilder.Entity<PostSlug>().ToContainer("PostSlugs").HasPartitionKey(postSlug => new { postSlug.ChannelId, postSlug.Slug });
        }
    }

    public class User : Entity
    {
        public required string Name { get; set; }
        public required string HashedPassword { get; set; }
        public required ICollection<UserOwnedEmail> Emails { get; set; }
        public required ICollection<UserOwnedSession> Sessions { get; set; }
        public required ICollection<UserOwnedSlug> Slugs { get; set; }
        public ICollection<UserOwnedInvite>? Invites { get; set; }
        public ICollection<UserOwnedChannel>?  Channels { get; set; }
        public required Guid DefaultSlugId { get; set; }
    }

    [Owned]
    public class UserOwnedChannel
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class UserOwnedEmail
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Address { get; set; }
        public required bool Verified { get; set; }
    }

    [Owned]
    public class UserOwnedSlug
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Slug { get; set; }
    }

    [Owned]
    public class UserOwnedInvite
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Code { get; set; }
        public required bool Used { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class UserOwnedSession
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
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
        public required ICollection<ChannelOwnedSlug> Slugs { get; set; }
        public ICollection<ChannelOwnedPost>? TopDraftPosts { get; set; }
        public ICollection<ChannelOwnedPost>? TopPublishedPosts { get; set; }
        public required Guid DefaultSlugId { get; set; }
    }

    [Owned]
    public class ChannelOwnedUser
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class ChannelOwnedSlug
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Slug { get; set; }
    }

    [Owned]
    public class ChannelOwnedPost
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public required DateTime UpdatedDateUTC { get; set; }
    }

    public class Post : Entity
    {
        public required string Name { get; set; }
        public required DateOnly DateNZ { get; set; }
        public required ICollection<PostOwnedSlug> Slugs { get; set; }
        public required Guid DefaultSlugId { get; set; }
        public required DateTime UpdatedDateUTC { get; set; }

        // content, an array of objects with type, value, caption, platform (youtube) etc
        // tags
    }

    [Owned]
    public class PostOwnedSlug
    {
        public required Guid Id { get; set; }
        public required Guid PostId { get; set; }
        public required string Slug { get; set; }
    }

    public class Event : Entity
    {
        public required Guid UserId { get; set; }
        public required string Type { get; set; }
        public required DateTime DateUtc { get; set; }
        public required IDictionary<string, string> Data { get; set; }
    }

    public class Email : Entity
    {
        public required string Address { get; set; }
        public required Guid UserId { get; set; }
        public string? VerificationCode { get; set; }
        public Guid? InvitedByUserId { get; set; }
        public Guid? InviteId { get; set; }
        public required bool Verified { get; set; }
    }

    public class Relationship : Entity
    {
        public required Guid UserId { get; set; }
        public required Guid ToUserId { get; set; }
        public required string Type { get; set; }
    }

    public class Invite : Entity
    {
        public required string Code { get; set; }
        public required Guid NewUserId { get; set; }
        public required Guid InvitedByUserId { get; set; }
        public required DateOnly DateNZ { get; set; }
        public string? EmailAddress { get; set; }
    }

    public class UserSlug : Entity
    {
        public required string Slug { get; set; }
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
    }

    public class ChannelSlug : Entity
    {
        public required string Slug { get; set; }
        public required Guid ChannelId { get; set; }
        public required string ChannelName { get; set; }
    }

    public class PostSlug : Entity
    {
        public required string Slug { get; set; }
        public required Guid ChannelId { get; set; }
        public required Guid PostId { get; set; }
        public required string PostName { get; set; }
    }

    public abstract class Entity
    {
        public Guid Id { get; set; }
    }
}
