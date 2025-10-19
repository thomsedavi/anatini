namespace Anatini.Server.Dtos
{
    public class ContentEditDto
    {
        public required Guid Id { get; set; }
        public required Guid ChannelId { get; set; }
        public required string Name { get; set; }
        public required string DefaultSlug { get; set; }
    }
}
