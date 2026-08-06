using System.ComponentModel.DataAnnotations;
using Anatini.Server.Utils;

namespace Anatini.Server.Posts
{
    public class CreateDocument
    {
        [MaxLength(64)]
        public required string Name { get; set; }

        [Handle, MaxLength(256)]
        public required string Handle { get; set; }
    }

    public class UpdateDocument
    {
        [MaxLength(64)]
        public string? Name { get; set; }

        public DateOnly? DateNZ { get; set; }

        public string? Status { get; set; }

        public string? Article { get;  set; }
    }
}
