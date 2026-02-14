using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Contents
{
    public class CreateContent
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }

        [Handle, MaxLength(64), DataType(DataType.Text)]
        public required string Handle { get; set; }

        public bool? Protected { get; set; }

        public string Id { get; set; } = RandomHex.NextX16();
    }

    public class UpdateContent
    {
        [MaxLength(64), DataType(DataType.Text)]
        public string? Name { get; set; }

        public DateOnly? DateNZ { get; set; }

        public string? Status { get; set; }

        public string? Article { get;  set; }
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
