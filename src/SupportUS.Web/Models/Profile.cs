namespace SupportUS.Web.Models
{
    public class Profile
    {
        public enum UserStatus
        {
            Admin,
            Banned,
            Basic,
            Developer,
            Premium
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public List<Quest> CreatedTasks { get; set; }

        public List<Quest> CompletedTasks { get; set; }

        public List<Quest> CurrentTasks { get; set; }

        public int Coins { get; set; }

        public double Rating { get; set; }

        public UserStatus Status { get; set; }
    }
}
