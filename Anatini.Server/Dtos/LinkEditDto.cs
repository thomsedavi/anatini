namespace Anatini.Server.Dtos
{
    public class LinkEditDto
    {
        public required Guid Id { get; set; }
        public string? Handle { get; set; }
        public required string Url { get; set; }
        public required string Article { get; set; }
        public required DateTime PublishedAtNz { get; set; }
        public required string Visibility { get; set; }
    }
}
