using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Contents
{
    public class CreateContent
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class UpdateContent
    {
        [MaxLength(64), DataType(DataType.Text)]
        public string? Name { get; set; }
        public DateOnly? DateNZ { get; set; }
        public string? Status { get; set; }
    }

    public class CreateElement
    {
        public required int InsertAfter { get; set; }
        [Tag, DataType(DataType.Text)]
        public required string Tag { get; set; }
        [DataType(DataType.Text)]
        public string? Content { get; set; }
    }

    public class UpdateElement
    {
        public required int Index { get; set; }
        [DataType(DataType.Text)]
        public required string Content { get; set; }
    }
}
