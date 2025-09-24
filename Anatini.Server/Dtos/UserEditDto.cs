namespace Anatini.Server.Dtos
{
    public class UserEditDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<UserEditChannelDto>? Channels { get; set; }
        public required IEnumerable<UserEditEmailDto> Emails { get; set; }
        public IEnumerable<UserEditInviteDto>? Invites { get; set; }
        public required IEnumerable<UserEditSessionDto> Sessions { get; set; }
        public required IEnumerable<UserEditSlugDto> Slugs { get; set; }
        public required Guid? DefaultSlugId { get; set; }
    }

    public class UserEditChannelDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }

    public class UserEditSlugDto
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
    }

    public class UserEditEmailDto
    {
        public required Guid Id { get; set; }
        public required string Address { get; set; }
        public required bool Verified { get; set; }
    }

    public class UserEditInviteDto
    {
        public required Guid Id { get; set; }
        public required string Code { get; set; }
        public required DateOnly DateNZ { get; set; }
        public required bool Used { get; set; }
    }

    public class UserEditSessionDto
    {
        public required Guid Id { get; set; }
        public required string UserAgent { get; set; }
        public required bool Revoked { get; set; }
        public required DateTime CreatedDateUtc { get; set; }
        public required DateTime UpdatedDateUtc { get; set; }
        public required string IPAddress { get; set; }
    }
}
