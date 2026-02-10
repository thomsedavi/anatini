namespace Anatini.Server.Dtos
{
    public class ChannelEditDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelEditContentDto>? TopDraftContents { get; set; }
        public required IEnumerable<ChannelEditAliasDto> Aliases { get; set; }
        public required string DefaultHandle { get; set; }
        public bool? Protected { get; set; }
        public string? DefaultCardImageId { get; set; }
    }

    public class ChannelEditContentDto
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
