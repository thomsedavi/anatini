namespace Anatini.Server.Dtos
{
    public class ChannelEditDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<ChannelEditPostDto>? TopDraftPosts { get; set; }
        public required IEnumerable<ChannelEditAliasDto> Aliases { get; set; }
        public required string DefaultSlug { get; set; }
    }

    public class ChannelEditPostDto
    {
        public required Guid Id { get; set; }
        public required string DefaultSlug { get; set; }
        public required string Name { get; set; }
        public required DateTime UpdatedDateTimeUTC { get; set; }
    }

    public class ChannelEditAliasDto
    {
        public required string Slug { get; set; }
    }
}
