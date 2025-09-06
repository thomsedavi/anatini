namespace Anatini.Server.Dtos
{
    internal class AccountDto(User user)
    {
        public string Name { get; } = user.Name;
        public IEnumerable<AccountEmailDto> Emails { get; } = user.Emails.Select(email => new AccountEmailDto(email));
        public IEnumerable<AccountInviteDto>? Invites { get; } = user.Invites?.Select(invite => new AccountInviteDto(invite));
        public IEnumerable<AccountRefreshTokenDto> RefreshTokens { get; } = user.RefreshTokens.Select(refreshToken => new AccountRefreshTokenDto(refreshToken));
        public IEnumerable<AccountHandleDto>? Handles { get; } = user.Handles?.Select(handle => new AccountHandleDto(handle));
        public Guid? DefaultHandleId { get; } = user.DefaultHandleId;
    }

    internal class AccountHandleDto(UserHandle handle)
    {
        public Guid HandleUserId { get; } = handle.HandleUserId;
        public string Handle { get; } = handle.Handle;
    }

    internal class AccountEmailDto(UserEmail email)
    {
        public Guid EmailUserId { get; } = email.EmailUserId;
        public string Email { get; } = email.Email;
        public bool Verified { get; } = email.Verified;
    }

    internal class AccountInviteDto(UserInvite invite)
    {
        public Guid CodeInviteId { get; } = invite.CodeInviteId;
        public string InviteCode { get; } = invite.InviteCode;
        public DateOnly CreatedDateNZ { get; } = invite.CreatedDateNZ;
        public bool Used { get; } = invite.Used;
    }

    internal class AccountRefreshTokenDto(UserRefreshToken refreshToken)
    {
        public string UserAgent { get; } = refreshToken.UserAgent;
        public bool Revoked { get; } = refreshToken.Revoked;
        public DateOnly CreatedDateNZ { get; } = refreshToken.CreatedDateNZ;
        public string IPAddress { get; } = refreshToken.IPAddress;
    }
}
