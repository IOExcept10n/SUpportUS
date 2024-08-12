using SupportUS.Web.Json;
using System.Text.Json.Serialization;

namespace SupportUS.Web.Models
{
    public class Review
    {
        public Guid Id { get; set; }

        [JsonConverter(typeof(ProfileShortConverter))]
        public Profile Author { get; set; }

        public string Content { get; set; }

        public Guid QuestId { get; set; }

        public int Rating { get; set; }
    }

    public record struct ReviewInfo(long AuthorId, Guid QuestId, string Content, int Rating);
}
