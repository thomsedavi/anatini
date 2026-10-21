using System.Text.Json;
using System.Text.Json.Serialization;

namespace Anatini.Server.Utils
{
    public class ShortDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions _)
        {
            return reader.GetDateTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions _)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm"));
        }
    }
}
