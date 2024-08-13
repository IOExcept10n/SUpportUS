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

        public enum CreationQuestStatus
        {
            None,
            Name,
            Description,
            Location,
            Price, 
            ExpectedDuration,
            Deadline
        }

        public long Id { get; set; }

        [JsonConverter(typeof(QuestListShortConverter))]
        public List<Quest> CreatedQuests { get; set; } = [];

        [JsonConverter(typeof(QuestListShortConverter))]
        public List<Quest> CompletedQuests { get; set; } = [];

        public int Coins { get; set; }

        public double Rating { get; set; }

        public CreationQuestStatus QuestStatus { get; set; }

        public ProfileStatus Status { get; set; }
    }
}
