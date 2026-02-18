namespace Anatini.Server.Dtos
{
    public class AttributeContentDto
    {
        public required string Name { get; set; }
        public required string ContentHandle { get; set; }
        public required string ContentChannelHandle { get; set; }
        public required DateOnly DateNZ { get; set; }
    }
}
