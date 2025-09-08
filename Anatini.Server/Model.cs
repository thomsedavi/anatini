using Microsoft.EntityFrameworkCore;

namespace Anatini.Server
{
    public class AnatiniContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Handle> Handles { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Relationship> Relationships { get; set; }
        public DbSet<Post> Posts { get; set; }

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
            var handleBuilder = modelBuilder.Entity<Handle>();

            userBuilder.ToContainer("Users").HasPartitionKey(user => user.Id);
            userBuilder.OwnsMany(user => user.Sessions, session => { session.HasKey("SessionId"); });
            userBuilder.OwnsMany(user => user.Emails, email => { email.HasKey("EmailId"); });
            userBuilder.OwnsMany(user => user.Handles, handle => { handle.HasKey("HandleId"); });
            userBuilder.OwnsMany(user => user.Invites, invite => { invite.HasKey("InviteId"); });

            handleBuilder.ToContainer("Handles").HasPartitionKey(user => user.Value);
            handleBuilder.OwnsMany(handle => handle.Users, user => { user.HasKey("UserId"); });

            modelBuilder.Entity<Event>().ToContainer("Events").HasPartitionKey(@event => @event.UserId);
            modelBuilder.Entity<Relationship>().ToContainer("Relationships").HasPartitionKey(relationship => relationship.UserId);
            modelBuilder.Entity<Post>().ToContainer("Posts").HasPartitionKey(post => post.HandleId);

            modelBuilder.Entity<Email>().ToContainer("Emails").HasPartitionKey(user => user.Value);
            modelBuilder.Entity<Invite>().ToContainer("Invites").HasPartitionKey(invite => invite.Value);
        }
    }

    public class User
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string HashedPassword { get; set; }
        public required ICollection<UserEmail> Emails { get; set; }
        public required ICollection<UserSession> Sessions { get; set; }
        public required ICollection<UserHandle> Handles { get; set; }
        public ICollection<UserInvite>? Invites { get; set; }
        public required Guid DefaultHandleId { get; set; }
    }

    [Owned]
    public class UserEmail
    {
        public required Guid EmailId { get; set; }
        public required string Value { get; set; }
        public required bool Verified { get; set; }
    }

    [Owned]
    public class UserHandle
    {
        public required Guid HandleId { get; set; }
        public required string Value { get; set; }
    }

    [Owned]
    public class UserInvite
    {
        public required Guid InviteId { get; set; }
        public required string Value { get; set; }
        public required bool Used { get; set; }
        public required DateOnly DateNZ { get; set; }
    }

    [Owned]
    public class UserSession
    {
        public required Guid SessionId { get; set; }
        public required string RefreshToken { get; set; }
        public required string IPAddress { get; set; }
        public required string UserAgent { get; set; }
        public required DateTime CreatedDateUtc { get; set; }
        public required DateTime UpdatedDateUtc { get; set; }
        public required bool Revoked { get; set; }
    }

    public class Post
    {
        public required Guid Id { get; set; }
        public required Guid HandleId { get; set; }
        public required string Title { get; set; }
        public required DateOnly DateNZ { get; set; }

        // content, an array of objects with type, value, caption, platform (youtube) etc
        // tags
    }

    public class Event
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Type { get; set; }
        public required DateTime DateUtc { get; set; }
        public required IDictionary<string, string> Data { get; set; }
    }

    public class Handle
    {
        public required Guid Id { get; set; }
        public required string Value { get; set; }
        public required ICollection<HandleUser> Users { get; set; }
    }

    [Owned]
    public class HandleUser
    {
        public required Guid UserId { get; set; }
        public required string UserName { get; set; }
    }

    public class Email
    {
        public required Guid Id { get; set; }
        public required string Value { get; set; }
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
        public required string Value { get; set; }
        public required Guid NewUserId { get; set; }
        public required Guid InvitedByUserId { get; set; }
        public required DateOnly DateNZ { get; set; }
    }
}
