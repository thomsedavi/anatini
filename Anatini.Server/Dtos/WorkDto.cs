namespace Anatini.Server.Dtos
{
    public class WorkDto : ContentDto
    {
        public required string Title { get; set; }
        public required string Visibility { get; set; }
        public DateTime? PublishedAtNz { get; set; }
    }
}
