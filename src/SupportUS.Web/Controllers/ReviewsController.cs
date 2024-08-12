using SupportUS.Web.Models;

namespace SupportUS.Web.Controllers
{
    public class ReviewsController(WebApplication app, APIControllers controllers)
        : ControllerBase(app, controllers)
    {
        public async Task PublishReviewAsync(HttpContext context)
        {
            var info = await context.Request.ReadFromJsonAsync<ReviewInfo>();
            using var db = GetDbContext();
            if (db.Reviews.Any(x => x.QuestId == info.QuestId))
            {
                await TellClientErrorAsync(context, "Cannot publish more than one review on the same quest.");
                return;
            }
            var quest = db.Quests.Find(info.QuestId);
            if (quest == null)
            {
                await TellNotFoundAsync(context, nameof(quest));
                return;
            }
            if (info.AuthorId != quest.Executor?.Id && info.AuthorId != quest.Customer.Id)
            {
                await TellClientErrorAsync(context, "Cannot publish review for the quest that you hadn't completed or made.");
                return;
            }
            var author = await db.Profiles.FindAsync(info.AuthorId);
            if (author == null)
            {
                await TellNotFoundAsync(context, nameof(author));
                return;
            }
            var review = new Review()
            {
                Id = Guid.NewGuid(),
                Author = author,
                Content = info.Content,
                QuestId = info.QuestId,
                Rating = info.Rating,
            };
            await db.Reviews.AddAsync(review);
            await db.SaveChangesAsync();
            await context.Response.WriteAsJsonAsync(review);
        }

        public async Task ReportReviewAsync(HttpContext context, Guid reviewId)
        {
            var ticket = await context.Request.ReadFromJsonAsync<Ticket>();
            if (ticket == null)
            {
                await TellClientErrorAsync(context, "Request doesn't have the ticket info.", StatusCodes.Status400BadRequest);
                return;
            }
            using var db = GetDbContext();
            var review = db.Reviews.Find(reviewId);
            if (review == null)
            {
                await TellNotFoundAsync(context, nameof(review));
                return;
            }
            await db.Tickets.AddAsync(ticket);
            await db.SaveChangesAsync();
        }
    }
}
