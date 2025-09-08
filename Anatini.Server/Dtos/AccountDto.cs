namespace Anatini.Server.Dtos
{
    internal class AccountDto(User user)
    {
        public Guid Id { get; } = user.Id;
        public string Name { get; } = user.Name;
        public IEnumerable<AccountEmailDto> Emails { get; } = user.Emails.Select(email => new AccountEmailDto(email));
        public IEnumerable<AccountInviteDto>? Invites { get; } = user.Invites?.Select(invite => new AccountInviteDto(invite));
        public IEnumerable<AccountSessionDto> Sessions { get; } = user.Sessions.Select(refreshToken => new AccountSessionDto(refreshToken));
        public IEnumerable<AccountHandleDto> Handles { get; } = user.Handles.Select(handle => new AccountHandleDto(handle));
        public Guid? DefaultHandleId { get; } = user.DefaultHandleId;
    }

    internal class AccountHandleDto(UserHandle handle)
    {
        public Guid HandleId { get; } = handle.HandleId;
        public string Value { get; } = handle.Value;
    }

    internal class AccountEmailDto(UserEmail email)
    {
        public Guid EmaiId { get; } = email.EmailId;
        public string Value { get; } = email.Value;
        public bool Verified { get; } = email.Verified;
    }

    internal class AccountInviteDto(UserInvite invite)
    {
        public Guid InviteId { get; } = invite.InviteId;
        public string Value { get; } = invite.Value;
        public DateOnly DateNZ { get; } = invite.DateNZ;
        public bool Used { get; } = invite.Used;
    }

    internal class AccountSessionDto(UserSession session)
    {
        public string UserAgent { get; } = session.UserAgent;
        public bool Revoked { get; } = session.Revoked;
        public DateTime CreatedDateUtc { get; } = session.CreatedDateUtc;
        public DateTime UpdatedDateUtc { get; } = session.UpdatedDateUtc;
        public string IPAddress { get; } = session.IPAddress;
    }
}
