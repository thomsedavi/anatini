using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Context.Entities
{
    public class User : IdentityUser<Guid>
    {
        public required string Name { get; set; }
        public string? About { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public required string Visibility { get; set; }

        public virtual ICollection<UserEmail> Emails { get; set; } = [];
        public virtual ICollection<UserSession> Sessions { get; set; } = [];
        public virtual ICollection<UserAlias> Aliases { get; set; } = [];
        public virtual ICollection<UserAction> Actions { get; set; } = [];
        public virtual ICollection<UserRole> Roles { get; set; } = [];
        public virtual ICollection<UserToken> Tokens { get; set; } = [];
        public virtual ICollection<UserLogin> Logins { get; set; } = [];
        public virtual ICollection<UserClaim> Claims { get; set; } = [];

        public UserEmail DefaultEmail => Emails.First(e => e.IsDefault);
        public UserAlias DefaultAlias => Aliases.First(e => e.IsDefault);

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserEmail : Entity
    {
        public required string Email { get; set; }
        public required string NormalizedEmail { get; set; }
        public string? ConfirmationCode { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public required bool IsDefault { get; set; }

        public Guid? UserId { get; set; }
        public virtual User? User { get; set; }
    }

    public class UserSession : Entity
    {
        public required string RefreshToken { get; set; }
        public required string IPAddress { get; set; }
        public required string UserAgent { get; set; }
        public required bool IsRevoked { get; set; } = false;

        public required Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }

    public class UserAlias : Entity
    {
        public required string Handle { get; set; }
        public required string NormalizedHandle { get; set; }
        public required bool IsDefault { get; set; }

        public required Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }

    public class UserAction
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string ActionType { get; set; }
        public DateTime DateTimeUtc { get; set; } = DateTime.UtcNow;
        public required UserEventData Data { get; set; }

        public required Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }

    public class UserEventData
    {
        public required string IPAddress { get; set; }
    }

    public class UserToUserRelationship
    {
        public required Guid Id { get; set; }
        public required Guid FromUserId { get; set; }
        public required Guid ToUserId { get; set; }
        public required string RelationshipType { get; set; }
    }

    public class UserImage
    {
        public required Guid Id { get; set; }
        public required string BlobContainerName { get; set; }
        public required string BlobName { get; set; }
        public string? AltText {  get; set; }
    }

    public class Role : IdentityRole<Guid>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; } = [];
        public virtual ICollection<RoleClaim> RoleClaims { get; set; } = [];
    }

    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }

    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual Role Role { get; set; } = null!;
    }

    public class UserClaim : IdentityUserClaim<Guid>
    {
        public virtual User User { get; set; } = null!;
    }

    public class UserLogin : IdentityUserLogin<Guid>
    {
        public virtual User User { get; set; } = null!;
    }

    public class UserToken : IdentityUserToken<Guid>
    {
        public virtual User User { get; set; } = null!;
    }
}
