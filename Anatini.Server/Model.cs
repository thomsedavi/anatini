using Microsoft.EntityFrameworkCore;

namespace Anatini.Server
{
    public class AnatiniContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<EmailUser> EmailUsers { get; set; }
        public DbSet<HandleUser> HandleUsers { get; set; }
        public DbSet<CodeInvite> CodeInvites { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<UserRelationship> UserRelationships { get; set; }

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

            userBuilder.ToContainer("Users").HasPartitionKey(user => user.Id);
            userBuilder.OwnsMany(user => user.Emails);
            userBuilder.OwnsMany(user => user.RefreshTokens);
            userBuilder.OwnsMany(user => user.Handles);
            userBuilder.OwnsMany(user => user.Invites);

            modelBuilder.Entity<UserEvent>().ToContainer("UserEvents").HasPartitionKey(userEvent => userEvent.UserId);
            modelBuilder.Entity<UserRelationship>().ToContainer("UserRelationships").HasPartitionKey(userEvent => userEvent.UserId);

            modelBuilder.Entity<EmailUser>().ToContainer("EmailUsers").HasPartitionKey(user => user.Email);
            modelBuilder.Entity<HandleUser>().ToContainer("HandleUsers").HasPartitionKey(user => user.Handle);
            modelBuilder.Entity<CodeInvite>().ToContainer("CodeInvites").HasPartitionKey(invite => invite.InviteCode);
        }
    }

    public class User
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string HashedPassword { get; set; }
        public required DateOnly CreatedDateNZ { get ; set; }
        public required ICollection<UserEmail> Emails { get; set; }
        public required ICollection<UserRefreshToken> RefreshTokens { get; set; }
        public required ICollection<UserHandle> Handles { get; set; }
        public ICollection<UserInvite>? Invites { get; set; }
        public required Guid DefaultHandleId { get; set; }
    }

    [Owned]
    public class UserEmail
    {
        public required Guid EmailUserId { get; set; }
        public required string Email { get; set; }
        public required bool Verified { get; set; }
    }

    [Owned]
    public class UserHandle
    {
        public required Guid HandleUserId { get; set; }
        public required string Handle { get; set; }
    }

    [Owned]
    public class UserInvite
    {
        public required Guid CodeInviteId { get; set; }
        public required string InviteCode { get; set; }
        public required bool Used { get; set; }
        public required DateOnly CreatedDateNZ { get; set; }
    }

    [Owned]
    public class UserRefreshToken
    {
        public required string RefreshToken { get; set; }
        public required string IPAddress { get; set; }
        public required string UserAgent { get; set; }
        public required DateOnly CreatedDateNZ { get; set; }
        public required bool Revoked { get; set; }
    }

    public class UserEvent
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Type { get; set; }
        public required DateTime DateTimeUtc { get; set; }
        public required IDictionary<string, string> Data { get; set; }
    }

    public class HandleUser
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Handle { get; set; }
    }

    public class EmailUser
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required Guid UserId { get; set; }
        public string? VerificationCode { get; set; }
        public Guid? InvitedByUserId { get; set; }
        public Guid? InviteId { get; set; }
        public required bool Verified { get; set; }
    }

    public class UserRelationship
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required Guid ToUserId { get; set; }
        public required string Type { get; set; }
    }

    public class CodeInvite
    {
        public required Guid Id { get; set; }
        public required string InviteCode { get; set; }
        public required Guid NewUserId { get; set; }
        public required Guid InvitedByUserId { get; set; }
        public required DateOnly CreatedDateNZ { get; set; }
    }
}
