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
            writer.WriteStringValue(value.Name);
        }
    }
}
