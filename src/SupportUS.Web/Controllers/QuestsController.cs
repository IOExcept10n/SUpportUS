using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using SupportUS.Web.Data;
using SupportUS.Web.Models;
using System.Runtime;

namespace SupportUS.Web.Controllers
{
    public class QuestsController(WebApplication app, APIControllers controllers) 
        : ControllerBase(app, controllers)
    {
        public async Task FindQuestByIdAsync(HttpContext context, Guid id)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var task = await db.Quests.FindAsync(id);
            var result = new
            {
                Found = task != null,
                Task = task
            };
            await context.Response.WriteAsJsonAsync(result);
        }

        public async Task CreateQuestAsync(HttpContext context)
        {
            var info = await context.Request.ReadFromJsonAsync<QuestInfo>();
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var customer = await db.Profiles.FindAsync(info.CustomerId);
            if (customer == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Quest customer not found.");
                return;
            }
            if (info.Name == null || 
                info.Description == null ||
                info.Price == null ||
                info.Location == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Some of required params are null. Name, description, price and location are required.");
                return;
            }
            var quest = Quest.CreateQuest(customer, info.Name, info.Description, info.Price.Value, info.Location);
            quest.ApplyInfo(info);
            await db.Quests.AddAsync(quest);
            await db.SaveChangesAsync();
            await context.Response.WriteAsJsonAsync(quest);
        }

        public async Task UpdateQuestAsync(HttpContext context)
        {
            var info = await context.Request.ReadFromJsonAsync<QuestInfo>();
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await db.Quests.FindAsync(info.Id);
            if (quest == null) 
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Quest not found.");
                return;
            }
            info.Name = null;
            info.Id = null;
            info.CustomerId = null;
            if (info.Price != null && quest.Status == Quest.QuestStatus.InProgress)
            {
                // To disallow changing price when quest is in completion state.
                info.Price = null;
            }
            quest.ApplyInfo(info);
            await db.SaveChangesAsync();
            await context.Response.WriteAsJsonAsync(quest);
        }

        public async Task DeleteQuestAsync(HttpContext context, Guid questId, long customerId)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await db.Quests.FindAsync(questId);
            if (quest == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Quest not found.");
                return;
            }
            if (quest.Customer.Id != customerId)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Cannot delete task when you're not its customer.");
                return;
            }
            db.Remove(quest);
            await db.SaveChangesAsync();
        }

        public async Task TakeQuestAsync(HttpContext context, Guid questId, long executorId)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await db.Quests.FindAsync(questId);
            if (quest == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Quest not found.");
                return;
            }
            if (quest.Status != Quest.QuestStatus.Opened)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Cannot take non-opened task.");
                return;
            }
            var executor = await db.Profiles.FindAsync(executorId);
            if (executor == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Executor profile not found.");
                return;
            }
            quest.Executor = executor;
            quest.Status = Quest.QuestStatus.InProgress;
            // TODO: notify.
            await db.SaveChangesAsync();
        }

        public async Task CancelQuestAsync(HttpContext context, Guid questId, long initiatorId)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await db.Quests.FindAsync(questId);
            if (quest == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Quest not found.");
                return;
            }
            if (quest.Status == Quest.QuestStatus.Completed)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Cannot cancel closed task.");
                return;
            }
            if (quest.Customer.Id != initiatorId)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Cannot cancel foreign task.");
                return;
            }
            quest.Status = Quest.QuestStatus.Cancelled;
            await db.SaveChangesAsync();
        }
    }
}
