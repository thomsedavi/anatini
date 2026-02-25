using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Posts
{
    public class CreateArticle
    {
        [MaxLength(64), DataType(DataType.Text)]
        public required string Name { get; set; }

        [Handle, MaxLength(64), DataType(DataType.Text)]
        public required string Handle { get; set; }

        public bool? Protected { get; set; }

        public string Id { get; set; } = RandomHex.NextX16();
    }

    public class UpdatePost
    {
        [MaxLength(64), DataType(DataType.Text)]
        public string? Name { get; set; }

        public DateOnly? DateNZ { get; set; }

        public string? Status { get; set; }

        public string? Article { get;  set; }
    }
}
