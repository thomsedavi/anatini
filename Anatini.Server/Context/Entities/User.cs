using Anatini.Server.Enums;
using Microsoft.AspNetCore.Identity;

namespace Anatini.Server.Context.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string Name { get; set; }
        public required string Handle { get; set; }
        public required string NormalizedHandle { get; set; }
        public string? About { get; set; }
        public Guid? IconImageId { get; set; }
        public Guid? BannerImageId { get; set; }
        public required Visibility Visibility { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUserImage? IconImage { get; set; }
        public virtual ApplicationUserImage? BannerImage { get; set; }

        public virtual ICollection<ApplicationUserEmail> Emails { get; set; } = [];
        public virtual ICollection<ApplicationUserImage> Images { get; set; } = [];
        public virtual ICollection<ApplicationUserHandle> Handles { get; set; } = [];
        public virtual ICollection<Log> Logs { get; set; } = [];
        public virtual ICollection<ApplicationUserRole> Roles { get; set; } = [];
        public virtual ICollection<ApplicationUserToken> Tokens { get; set; } = [];
        public virtual ICollection<ApplicationUserLogin> Logins { get; set; } = [];
        public virtual ICollection<ApplicationUserClaim> Claims { get; set; } = [];
        public virtual ICollection<ApplicationUserTrust> GivenTrusts { get; set; } = [];
        public virtual ICollection<ApplicationUserTrust> ReceivedTrusts { get; set; } = [];
        public virtual ICollection<ApplicationUserChannel> UserChannels { get; set; } = [];
    }

    public class ApplicationUserEmail
    {
        public required Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public required string Email { get; set; }
        public required string NormalizedEmail { get; set; }
        public string? ConfirmationCode { get; set; }
        public required bool EmailConfirmed { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

        public virtual ApplicationUser? User { get; set; }
    }

    public class ApplicationUserHandle
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string Handle { get; set; }
        public required string NormalizedHandle { get; set; }
        public required DateTime CreatedAtUtc { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
    }

    public class ApplicationUserChannel
    {
        public required Guid UserId { get; set; }
        public required Guid ChannelId { get; set; }
        public required DateTime CreatedAtUtc { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Channel Channel { get; set; } = null!;
    }

    public class ApplicationUserTrust
    {
        public required Guid SourceUserId { get; set; }
        public required Guid TargetUserId { get; set; }
        public required DateTime CreatedAtUtc { get; set; }

        public virtual ApplicationUser SourceUser { get; set; } = null!;
        public virtual ApplicationUser TargetUser { get; set; } = null!;
    }

    public class ApplicationUserImage
    {
        public required Guid Id { get; set; }
        public required Guid UserId { get; set; }
        public required string BlobName { get; set; }
        public required string BlobContainerName { get; set; }
        public string? AltText { get; set; }
        public required DateTime CreatedAtUtc { get; set; }
        public required DateTime UpdatedAtUtc { get; set; }

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
