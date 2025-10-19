using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Contents
{
    public class NewContent
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }
        [Slug, MaxLength(64), DataType(DataType.Text)]
        public required string Slug { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class NewElement
    {
        public required int InsertAfter { get; set; }
        [Tag, DataType(DataType.Text)]
        public required string Tag { get; set; }
        [DataType(DataType.Text)]
        public string? Content { get; set; }
    }
}
