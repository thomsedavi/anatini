using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Context.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string DisplayName { get; set; }
        public string? About { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public required string Visibility { get; set; }

        public virtual ICollection<ApplicationUserEmail> Emails { get; set; } = [];
        public virtual ICollection<ApplicationUserImage> Images { get; set; } = [];
        public virtual ICollection<ApplicationUserName> Aliases { get; set; } = [];
        public virtual ICollection<ApplicationUserEvent> Events { get; set; } = [];
        public virtual ICollection<ApplicationUserRole> Roles { get; set; } = [];
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; } = [];
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; } = [];
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; } = [];

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ApplicationUserEmail : Entity
    {
        public required string Email { get; set; }
        public required string NormalizedEmail { get; set; }
        public string? ConfirmationCode { get; set; }
        public bool EmailConfirmed { get; set; } = false;

        public Guid? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }

    public class ApplicationUserName : Entity
    {
        public required string UserName { get; set; }
        public required string NormalizedUserName { get; set; }

        public required Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public class ApplicationUserEvent
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Type { get; set; }
        public DateTime DateTimeUtc { get; set; } = DateTime.UtcNow;
        public required UserEventData Data { get; set; }

        public required Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public class UserEventData
    {
        public required string IPAddress { get; set; }
    }

    public class ApplicationUserToUserRelationship
    {
        public required Guid Id { get; set; }
        public required Guid FromUserId { get; set; }
        public required Guid ToUserId { get; set; }
        public required string RelationshipType { get; set; }
    }

    public class ApplicationUserImage : Entity
    {
        public required string BlobContainerName { get; set; }
        public required string BlobName { get; set; }
        public string? AltText {  get; set; }

        public required Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = [];
        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = [];
    }

    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ApplicationRole Role { get; set; } = null!;
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual ApplicationRole Role { get; set; } = null!;
    }

    public class ApplicationUserClaim : IdentityUserClaim<Guid>
    {
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public class ApplicationUserLogin : IdentityUserLogin<Guid>
    {
        public virtual ApplicationUser User { get; set; } = null!;
    }

    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
