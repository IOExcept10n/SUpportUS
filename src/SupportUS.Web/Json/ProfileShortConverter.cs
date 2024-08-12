using SupportUS.Web.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SupportUS.Web.Json
{
    public class ProfileShortConverter : JsonConverter<Profile>
    {
        public override Profile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Profile value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Id.ToString());
        }
    }
}
