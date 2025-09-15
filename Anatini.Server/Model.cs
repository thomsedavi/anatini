using Microsoft.EntityFrameworkCore;

namespace Anatini.Server
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
            userBuilder.OwnsMany(user => user.Sessions, session => { session.WithOwner().HasForeignKey("UserId"); session.HasKey("SessionId"); });
            userBuilder.OwnsMany(user => user.Emails, email => { email.WithOwner().HasForeignKey("UserId"); email.HasKey("EmailId"); });
            userBuilder.OwnsMany(user => user.Invites, invite => { invite.WithOwner().HasForeignKey("UserId"); invite.HasKey("InviteId"); });
            userBuilder.OwnsMany(user => user.Channels, channel => { channel.WithOwner().HasForeignKey("UserId"); channel.HasKey("ChannelId"); });
            userBuilder.OwnsMany(user => user.Slugs, slug => { slug.WithOwner().HasForeignKey("UserId"); slug.HasKey("SlugId"); });

            channelBuilder.ToContainer("Channels").HasPartitionKey(channel => channel.Id);
            channelBuilder.OwnsMany(channel => channel.Users, slug => { slug.WithOwner().HasForeignKey("ChannelId"); slug.HasKey("UserId"); });
            channelBuilder.OwnsMany(channel => channel.Slugs, slug => { slug.WithOwner().HasForeignKey("ChannelId"); slug.HasKey("SlugId"); });

            postBuilder.ToContainer("Posts").HasPartitionKey(post => post.Id);
            postBuilder.OwnsMany(post => post.Slugs, slug => { slug.WithOwner().HasForeignKey("PostId"); slug.HasKey("SlugId"); });

            modelBuilder.Entity<Event>().ToContainer("Events").HasPartitionKey(@event => @event.UserId);
            modelBuilder.Entity<Relationship>().ToContainer("Relationships").HasPartitionKey(relationship => relationship.UserId);

            modelBuilder.Entity<Email>().ToContainer("Emails").HasPartitionKey(email => email.Address);
            modelBuilder.Entity<Invite>().ToContainer("Invites").HasPartitionKey(invite => invite.Code);

            modelBuilder.Entity<UserSlug>().ToContainer("UserSlugs").HasPartitionKey(userSlug => userSlug.Slug);
            modelBuilder.Entity<ChannelSlug>().ToContainer("ChannelSlugs").HasPartitionKey(channelSlug => channelSlug.Slug);
            modelBuilder.Entity<PostSlug>().ToContainer("PostSlugs").HasPartitionKey(postSlug => postSlug.ChannelId);
        }
    }

    public class User
    {
        public required Guid Id { get; set; }
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
        public required Guid ChannelId { get; set; }
        public required Guid UserId { get; set; }
        public required string Name { get; set; }
    }

    [Owned]
    public class UserOwnedEmail
    {
        public required Guid EmailId { get; set; }
        public required Guid UserId { get; set; }
        public required string Address { get; set; }
        public required bool Verified { get; set; }
    }

    [Owned]
    public class UserOwnedSlug
    {
        public required Guid SlugId { get; set; }
        public required Guid UserId { get; set; }
        public required string Slug { get; set; }
    }

    [Owned]
    public class UserOwnedInvite
    {
        public required Guid InviteId { get; set; }
        public required Guid UserId { get; set; }
        public required string Code { get; set; }
        public required bool Used { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class UserOwnedSession
    {
        public required Guid SessionId { get; set; }
        public required Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
        public required string IPAddress { get; set; }
        public required string UserAgent { get; set; }
        public required DateTime CreatedDateUtc { get; set; }
        public required DateTime UpdatedDateUtc { get; set; }
        public required bool Revoked { get; set; }
    }

    public class Channel
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required ICollection<ChannelOwnedUser> Users { get; set; }
        public required ICollection<ChannelOwnedSlug> Slugs { get; set; }
    }

    [Owned]
    public class ChannelOwnedUser
    {
        public required Guid UserId { get; set; }
        public required Guid ChannelId { get; set; }
        public required string UserName { get; set; }
    }

    [Owned]
    public class ChannelOwnedSlug
    {
        public required Guid SlugId { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Slug { get; set; }
    }

    public class Post
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required DateOnly DateNZ { get; set; }
        public required ICollection<PostOwnedSlug> Slugs { get; set; }

        // content, an array of objects with type, value, caption, platform (youtube) etc
        // tags
    }

    [Owned]
    public class PostOwnedSlug
    {
        public required Guid SlugId { get; set; }
        public required Guid PostId { get; set; }
        public required string Slug { get; set; }
    }

    public class Event
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Type { get; set; }
        public required DateTime DateUtc { get; set; }
        public required IDictionary<string, string> Data { get; set; }
    }

    public class Email
    {
        public required Guid Id { get; set; }
        public required string Address { get; set; }
        public required Guid UserId { get; set; }
        public string? VerificationCode { get; set; }
        public Guid? InvitedByUserId { get; set; }
        public Guid? InviteId { get; set; }
        public required bool Verified { get; set; }
    }

    public class Relationship
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid ToUserId { get; set; }
        public required string Type { get; set; }
    }

    public class Invite
    {
        public required Guid Id { get; set; }
        public required string Code { get; set; }
        public required Guid NewUserId { get; set; }
        public required Guid InvitedByUserId { get; set; }
        public required DateOnly DateNZ { get; set; }
        public string? EmailAddress { get; set; }
    }

    public class UserSlug
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
    }

    public class ChannelSlug
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
        public required Guid ChannelId { get; set; }
        public required string ChannelName { get; set; }
    }

    public class PostSlug
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
        public required Guid ChannelId { get; set; }
        public required Guid PostId { get; set; }
        public required string PostName { get; set; }
    }
}
