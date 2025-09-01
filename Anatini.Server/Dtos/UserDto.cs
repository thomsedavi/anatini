namespace Anatini.Server.Dtos
{
    internal class UserDto(User user)
    {
        public string Name { get; } = user.Name;
        public IEnumerable<UserEmailDto> Emails { get; } = user.Emails.Select(email => new UserEmailDto(email));
        public IEnumerable<UserInviteDto>? Invites { get; } = user.Invites?.Select(invite => new UserInviteDto(invite));
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
}
