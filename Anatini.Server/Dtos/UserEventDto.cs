namespace Anatini.Server.Dtos
{
    internal class UserEventDto(UserEvent userEvent)
    {
        public string Type { get; } = userEvent.Type;
        public DateTime DateTimeUtc { get; } = userEvent.DateTimeUtc;
    }
}
