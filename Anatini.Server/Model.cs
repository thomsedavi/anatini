using Microsoft.EntityFrameworkCore;

namespace Anatini.Server
{
    public class AnatiniContext() : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<EmailUser> EmailUsers { get; set; }
        public DbSet<Alias> Aliases { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }

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
            modelBuilder.Entity<User>().ToContainer("Users").HasPartitionKey(user => user.Id);
            modelBuilder.Entity<EmailUser>().ToContainer("EmailUsers").HasPartitionKey(user => user.Email);
            modelBuilder.Entity<Alias>().ToContainer("Alias").HasPartitionKey(user => user.UserId);
            modelBuilder.Entity<Post>().ToContainer("Posts").HasPartitionKey(post => post.UserId);
            modelBuilder.Entity<Invite>().ToContainer("Invites").HasPartitionKey(post => post.UserId);
            modelBuilder.Entity<UserEvent>().ToContainer("UserEvents").HasPartitionKey(post => post.UserId);
        }
    }

    public class User
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string HashedPassword { get; set; }
        public required DateOnly CreatedDate { get ; set; }
        public required IEnumerable<UserEmail> Emails { get; set; }
    }

    public class UserEmail
    {
        public required string Email { get; set; }
        public required bool IsVerified { get; set; }
    }

    public class EmailUser
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required Guid UserId { get; set; }
        public string? VerificationCode { get; set; }
    }

    public class UserEvent
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Type { get; set; }
        public required DateTime DateTimeUtc { get; set; }
        public required IDictionary<string, string> Details { get; set; }
    }

    public class Alias
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public bool? IsPrimary { get; set; }
        public required string UserId { get; set; }
    }

    public class Post
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string UserId { get; set; }
    }

    public class Invite
    {
        public required Guid Id { get; set; }
        public required string Code { get; set; }
        public required string UserId { get; set; }
    }
}
