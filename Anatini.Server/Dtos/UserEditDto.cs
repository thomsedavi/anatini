namespace Anatini.Server.Dtos
{
    public class UserEditDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string DefaultSlug { get; set; }
        public string? Bio { get; set; }
        public IEnumerable<UserEditChannelDto>? Channels { get; set; }
        public required IEnumerable<UserEditEmailDto> Emails { get; set; }
        public IEnumerable<UserEditSessionDto>? Sessions { get; set; }
        public required IEnumerable<UserEditAliasDto> Aliases { get; set; }
        public IList<string>? Permissions { get;  set; }
        public bool? Protected { get; set; }
    }

    public class UserEditChannelDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string DefaultSlug { get; set; }
    }

    public class UserEditAliasDto
    {
        public required string Slug { get; set; }
    }

    public class UserEditEmailDto
    {
        public required string Address { get; set; }
        public required bool Verified { get; set; }
    }

    public class UserEditSessionDto
    {
        public required Guid Id { get; set; }
        public required string UserAgent { get; set; }
        public required bool Revoked { get; set; }
        public required DateTime CreatedDateTimeUtc { get; set; }
        public required DateTime UpdatedDateTimeUtc { get; set; }
        public required string IPAddress { get; set; }
    }
}
