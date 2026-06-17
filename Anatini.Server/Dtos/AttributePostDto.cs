namespace Anatini.Server.Dtos
{
    public class AttributePostDto
    {
        public required string Name { get; set; }
        public required string PostHandle { get; set; }
        public required string PostSpaceHandle { get; set; }
        public required DateOnly DateNZ { get; set; }
    }
}
