namespace SupportUS.Web.Models
{
    public class Review
    {
        public Guid Id { get; set; }

        public Profile Author { get; set; }

        public string Content { get; set; }
    }
}
