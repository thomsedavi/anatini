namespace Anatini.Server.Dtos
{
    public class ChannelEditDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelEditPostDto>? TopDraftPosts { get; set; }
        public required IEnumerable<ChannelEditAliasDto> Aliases { get; set; }
        public required string DefaultHandle { get; set; }
        public bool? Protected { get; set; }
        public string? DefaultCardImageId { get; set; }
    }

    public class ChannelEditPostDto
    {
        public required string Id { get; set; }
        public required string DefaultHandle { get; set; }
        public required string Name { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
    }

    public class ChannelEditAliasDto
    {
        public required string Handle { get; set; }
    }
}
