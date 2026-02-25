namespace Anatini.Server.Dtos
{
    public class PostEditDto
    {
        public required string Id { get; set; }
        public required string ChannelId { get; set; }
        public required string DefaultHandle { get; set; }
        public required PostVersionDto Version { get; set; }
        public bool? Protected { get; set; }
        public required string Status { get; set; }
    }

    public class PostVersionDto
    {
        public required string Name { get; set; }
        public string? Article { get; set; }
        public required DateOnly DateNZ { get; set; }
    }
}
