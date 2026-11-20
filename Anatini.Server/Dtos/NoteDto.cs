namespace Anatini.Server.Dtos
{
    public class NoteDto : ContentDto
    {
        public string? Title { get; set; }
        public DateTime PublishedAtNz { get; set; }
    }
}
