using Anatini.Server.Utils;

namespace Anatini.Server.Notes
{
    public class CreateNote
    {
        public required string Article { get; set; }

        public bool? Protected { get; set; }

        public string Id { get; set; } = RandomHex.NextX16();
    }
}
