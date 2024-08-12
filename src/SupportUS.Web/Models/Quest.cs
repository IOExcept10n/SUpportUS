namespace SupportUS.Web.Models
{
    public class Quest()
    {
        public static Quest CreateQuest(Profile customer, string name, string description, int price, string location)
        {
            return new()
            {
                Customer = customer,
                Name = name,
                Description = description,
                Price = price,
                Location = location,
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow,
            };
        }

        public enum QuestStatus
        {
            Draft,
            Opened,
            InProgress,
            Completed,
            Cancelled,
        }

        public Guid Id { get; set; }

        public required Profile Customer { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public QuestStatus Status { get; set; }

        public int Price { get; set; }

        public required string Location { get; set; }

        public TimeSpan? ExpectedDuration { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime CreationDate { get; set; }

        public Review? ExecutorReview { get; set; }

        public Review? CustomerReview { get; set; }

        public void ApplyInfo(QuestInfo info)
        {
            Id = info.Id ?? Id;
            Name = info.Name ?? Name;
            Description = info.Description ?? Description;
            Price = info.Price ?? Price;
            Location = info.Location ?? Location;
            ExpectedDuration = info.ExpectedDuration ?? ExpectedDuration;
            Deadline = info.Deadline ?? Deadline;
        }
    }

    public record struct QuestInfo(Guid? Id,
                                   Guid? CustomerId,
                                   string? Name,
                                   string? Description,
                                   int? Price,
                                   string? Location, 
                                   TimeSpan? ExpectedDuration,
                                   DateTime? Deadline);

}
