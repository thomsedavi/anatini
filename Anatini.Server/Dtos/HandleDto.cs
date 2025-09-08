namespace Anatini.Server.Dtos
{
    internal class HandleDto(Handle handle)
    {
        public Guid Id { get; } = handle.Id;
        public string Value { get; } = handle.Value;
        public IEnumerable<HandleUserDto> Users { get; } = handle.Users.Select(user => new HandleUserDto(user));
    }

    internal class HandleUserDto(HandleUser user)
    {
        public Guid UserId { get; } = user.UserId;
        public string UserName { get; } = user.UserName;
    }
}
