using SupportUS.Web.Data;
using SupportUS.Web.Models;

namespace SupportUS.Web.Controllers
{
    public class ReviewsController(WebApplication app, APIControllers controllers)
        : ControllerBase(app, controllers)
    {
        public async Task PublishReview(HttpContext context)
        {
            var info = await context.Request.ReadFromJsonAsync<ReviewInfo>();
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            if (db.Reviews.Any(x => x.QuestId == info.QuestId)) 
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Cannot publish more than one review on one quest.");
                return;
            }
            var quest = db.Quests.Find(info.QuestId);
            if (quest == null)
            {

            }
        }
    }
}
