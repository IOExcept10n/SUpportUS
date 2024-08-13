using SupportUS.Web.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SupportUS.Web.Json
{
    public class QuestShortConverter : JsonConverter<Quest>
    {
        public override Quest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Quest value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id);
        }
    }

    public class QuestListShortConverter : JsonConverter<List<Quest>>
    {
        public override List<Quest>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return [];
        }

        public override void Write(Utf8JsonWriter writer, List<Quest> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var item in value)
            {
                writer.WriteStringValue(item.Id);
            }
            writer.WriteEndArray();
        }
    }
}
