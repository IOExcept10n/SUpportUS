namespace SupportUS.Web.Models
{
    public class Quest
    {
        public enum QuestStatus
        {
            Draft,
            Opened,
            InProgress,
            Completed,
            Cancelled,
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public QuestStatus Status { get; set; }

        public int Price { get; set; }

        public string Location { get; set; }

        public TimeSpan? ExpectedDuration { get; set; }

        public DateTime? Deadline { get; set; }

        public Review? ContractorReview { get; set; }

        public Review? CustomerReview { get; set; }
    }
}
