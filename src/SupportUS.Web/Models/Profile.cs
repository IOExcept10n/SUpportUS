using SupportUS.Web.Json;
using System.Text.Json.Serialization;

namespace SupportUS.Web.Models
{
    public class Profile
    {
        public enum ProfileStatus
        {
            Admin,
            Banned,
            Basic,
            Developer,
            Premium
        }

        public long Id { get; set; }

        [JsonConverter(typeof(QuestShortConverter))]
        public List<Quest> CreatedQuests { get; set; } = [];

        [JsonConverter(typeof(QuestShortConverter))]
        public List<Quest> CompletedQuests { get; set; } = [];

        public int Coins { get; set; }

        public double Rating { get; set; }

        public ProfileStatus Status { get; set; }
    }
}
