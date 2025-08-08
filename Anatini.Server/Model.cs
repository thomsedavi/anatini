using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Anatini.Server
{
    public class AnatiniContext() : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Alias> Aliases { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Invite> Invites { get; set; }

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
            modelBuilder.Entity<Alias>().ToContainer("Alias").HasPartitionKey(user => user.UserId);
            modelBuilder.Entity<Post>().ToContainer("Posts").HasPartitionKey(post => post.UserId);
            modelBuilder.Entity<Invite>().ToContainer("Invites").HasPartitionKey(post => post.UserId);
        }
    }

    public class User
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }

        public List<Alias> Aliases { get; } = [];
        public List<Post> Posts { get; } = [];
        public List<Invite> Invites { get; } = [];
    }

    public class Alias
    {
        public required Guid Id { get; set; }
        public required string Handle { get; set; }
        public bool? IsPrimary { get; set; }
        public int UserId { get; set; }
    }

    public class Post
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public int UserId { get; set; }
    }

    public class Invite
    {
        public required Guid Id { get; set; }
        public int Code { get; set; }
        public int UserId { get; set; }
    }
}
