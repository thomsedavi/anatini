namespace Anatini.Server.Dtos
{
    internal class AccountDto(User user)
    {
        public Guid Id { get; } = user.Id;
        public string Name { get; } = user.Name;
        public IEnumerable<AccountChannelDto> Channels { get; } = user.Channels?.Select(channel => new AccountChannelDto(channel)) ?? [];
        public IEnumerable<AccountEmailDto> Emails { get; } = user.Emails.Select(email => new AccountEmailDto(email));
        public IEnumerable<AccountInviteDto> Invites { get; } = user.Invites?.Select(invite => new AccountInviteDto(invite)) ?? [];
        public IEnumerable<AccountSessionDto> Sessions { get; } = user.Sessions.Select(refreshToken => new AccountSessionDto(refreshToken));
        public IEnumerable<AccountSlugDto> Slugs { get; } = user.Slugs.Select(slug => new AccountSlugDto(slug));
        public Guid? DefaultSlugId { get; } = user.DefaultSlugId;
    }

    internal class AccountChannelDto(UserOwnedChannel channel)
    {
        public Guid ChannelId { get; } = channel.ChannelId;
        public string Name { get; } = channel.Name;
    }

    internal class AccountSlugDto(UserOwnedSlug slug)
    {
        public Guid SlugId { get; } = slug.SlugId;
        public string Slug { get; } = slug.Slug;
    }

    internal class AccountEmailDto(UserOwnedEmail email)
    {
        public Guid EmaiId { get; } = email.EmailId;
        public string Address { get; } = email.Address;
        public bool Verified { get; } = email.Verified;
    }

    internal class AccountInviteDto(UserOwnedInvite invite)
    {
        public Guid InviteId { get; } = invite.InviteId;
        public string Code { get; } = invite.Code;
        public DateOnly DateNZ { get; } = invite.DateNZ;
        public bool Used { get; } = invite.Used;
    }

    internal class AccountSessionDto(UserOwnedSession session)
    {
        public string UserAgent { get; } = session.UserAgent;
        public bool Revoked { get; } = session.Revoked;
        public DateTime CreatedDateUtc { get; } = session.CreatedDateUtc;
        public DateTime UpdatedDateUtc { get; } = session.UpdatedDateUtc;
        public string IPAddress { get; } = session.IPAddress;
    }
}
