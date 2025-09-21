namespace Anatini.Server.Dtos
{
    public class AccountDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<AccountChannelDto>? Channels { get; set; }
        public required IEnumerable<AccountEmailDto> Emails { get; set; }
        public IEnumerable<AccountInviteDto>? Invites { get; set; }
        public required IEnumerable<AccountSessionDto> Sessions { get; set; }
        public required IEnumerable<AccountSlugDto> Slugs { get; set; }
        public required Guid? DefaultSlugId { get; set; }
    }

    public class AccountChannelDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
    }

    public class AccountSlugDto
    {
        public required Guid Id { get; set; }
        public required string Slug { get; set; }
    }

    public class AccountEmailDto
    {
        public required Guid Id { get; set; }
        public required string Address { get; set; }
        public required bool Verified { get; set; }
    }

    public class AccountInviteDto
    {
        public required Guid Id { get; set; }
        public required string Code { get; set; }
        public required DateOnly DateNZ { get; set; }
        public required bool Used { get; set; }
    }

    public class AccountSessionDto
    {
        public required Guid Id { get; set; }
        public required string UserAgent { get; set; }
        public required bool Revoked { get; set; }
        public required DateTime CreatedDateUtc { get; set; }
        public required DateTime UpdatedDateUtc { get; set; }
        public required string IPAddress { get; set; }
    }
}
