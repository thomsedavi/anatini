namespace Anatini.Server.Dtos
{
    internal class UserDto(User user)
    {
        public string Name { get; } = user.Name;
        public IEnumerable<UserEmailDto> Emails { get; } = user.Emails.Select(email => new UserEmailDto(email));
        public IEnumerable<UserInviteDto>? Invites { get; } = user.Invites?.Select(invite => new UserInviteDto(invite));
        public IEnumerable<RefreshTokenDto> RefreshTokens { get; } = user.RefreshTokens.Select(refreshToken => new RefreshTokenDto(refreshToken));
    }

    internal class UserEmailDto(UserEmail email)
    {
        public string Email { get; } = email.Email;
        public bool Verified { get; } = email.Verified;
    }

    internal class UserInviteDto(UserInvite invite)
    {
        public string InviteCode { get; } = invite.InviteCode;
        public DateOnly CreatedDateNZ { get; } = invite.CreatedDateNZ;
        public bool Used { get; } = invite.Used;
    }

    internal class RefreshTokenDto(UserRefreshToken refreshToken)
    {
        public string UserAgent { get; } = refreshToken.UserAgent;
        public bool Revoked { get; } = refreshToken.Revoked;
        public DateOnly CreatedDateNZ { get; } = refreshToken.CreatedDateNZ;
        public string IPAddress { get; } = refreshToken.IPAddress;
    }
}
