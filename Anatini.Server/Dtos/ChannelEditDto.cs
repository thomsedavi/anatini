namespace Anatini.Server.Dtos
{
    public class ChannelEditDto
    {
        public required Guid Guid { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelEditPostDto>? TopDraftPosts { get; set; }
        public required IEnumerable<ChannelEditAliasDto> Aliases { get; set; }
        public required Guid? DefaultAliasGuid { get; set; }
    }

    public class ChannelEditPostDto
    {
        public required Guid Guid { get; set; }
        public required string Slug { get; set; }
        public required string Name { get; set; }
        public required DateTime UpdatedDateUTC { get; set; }
    }

    public class ChannelEditAliasDto
    {
        public required Guid Guid { get; set; }
        public required string Slug { get; set; }
    }
}
