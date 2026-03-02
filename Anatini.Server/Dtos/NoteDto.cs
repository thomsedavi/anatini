namespace Anatini.Server.Dtos
{
    public class NoteDto
    {
        public required string Article { get; set; }
        public required DateTime DateTimeUTC { get; set; }
    }
}
