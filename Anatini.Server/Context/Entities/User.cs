using Microsoft.EntityFrameworkCore;

namespace Anatini.Server.Context.Entities
{
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
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public bool? Protected { get; set; }
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
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public bool? Protected { get; set; }
    }

    public class UserImage : UserEntity
    {
        public required Guid Id { get; set; }
        public required string BlobContainerName { get; set; }
        public required string BlobName { get; set; }
        public string? AltText {  get; set; }
    }

    public abstract class UserEntity : Entity
    {
        public required Guid UserId { get; set; }
    }

    public abstract class UserOwnedEntity
    {
        public required Guid UserId { get; set; }
    }
}
