using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using SupportUS.Web.Data;
using SupportUS.Web.Models;
using System.Runtime;

namespace SupportUS.Web.Controllers
{
    public class QuestsController(WebApplication app, APIControllers controllers) 
        : ControllerBase(app, controllers)
    {
        public async Task FindQuestById(HttpContext context, Guid id)
        {
            var db = Application.Services.GetRequiredService<AppDbContext>();
            var task = await db.Quests.FindAsync(id);
            var result = new
            {
                Found = task != null,
                Task = task
            };
            await context.Response.WriteAsJsonAsync(result);
        }

        public async Task CreateQuest(HttpContext context)
        {
            var info = await context.Request.ReadFromJsonAsync<QuestInfo>();
            var db = Application.Services.GetRequiredService<AppDbContext>();
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
            await context.Response.WriteAsJsonAsync(quest);
        }

        public async Task UpdateQuest(HttpContext context)
        {
            var info = await context.Request.ReadFromJsonAsync<QuestInfo>();
            var db = Application.Services.GetRequiredService<AppDbContext>();
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
            db.Update(quest);
            await context.Response.WriteAsJsonAsync(quest);
        }

        public async Task DeleteQuest(HttpContext context, Guid questId, Guid customerId)
        {
            var db = Application.Services.GetRequiredService<AppDbContext>();
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
                await context.Response.WriteAsync("Cannot delete task whose custome.");
                return;
            }
            db.Remove(quest);
        }
    }
}
