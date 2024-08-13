using SupportUS.Web.Bot;
using SupportUS.Web.Models;

namespace SupportUS.Web.API
{
    public class QuestsController(WebApplication app, APIControllers controllers) 
        : ControllerBase(app, controllers)
    {
        public async Task FindQuestByIdAsync(HttpContext context, Guid id)
        {
            using var db = GetDbContext();
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
            using var db = GetDbContext();
            var customer = await db.Profiles.FindAsync(info.CustomerId);
            if (customer == null)
            {
                await TellNotFoundAsync(context, nameof(customer));
                return;
            }
            if (info.Name == null || 
                info.Description == null ||
                info.Price == null ||
                info.Location == null)
            {
                await TellClientErrorAsync(context, "Some of required params are null. Name, description, price and location are required.", StatusCodes.Status400BadRequest);
                return;
            }
            if (customer.Coins < info.Price)
            {
                await TellClientErrorAsync(context, "Customer doesn't have enough money to open quest.");
                return;
            }
            var quest = Quest.CreateQuest(customer, info.Name, info.Description, info.Price.Value, info.Location);
            quest.ApplyInfo(info);
            quest.Status = Quest.QuestStatus.Opened;
            customer.CreatedQuests.Add(quest);
            await db.Quests.AddAsync(quest);
            await db.SaveChangesAsync();
            Application.Services.GetService<BotService>()?.MailingService.MailMessageQuest(quest);
            await context.Response.WriteAsJsonAsync(quest);
        }

        public async Task UpdateQuestAsync(HttpContext context)
        {
            var info = await context.Request.ReadFromJsonAsync<QuestInfo>();
            using var db = GetDbContext();
            var quest = await db.Quests.FindAsync(info.Id);
            if (quest == null) 
            {
                await TellNotFoundAsync(context, nameof(quest));
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
            using var db = GetDbContext();
            var quest = await db.Quests.FindAsync(questId);
            if (quest == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Quest not found.");
                return;
            }
            if (quest.CustomerId != customerId)
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
            using var db = GetDbContext();
            var quest = await db.Quests.FindAsync(questId);
            if (quest == null)
            {
                await TellNotFoundAsync(context, nameof(quest));
                return;
            }

            if (quest!.Status != Quest.QuestStatus.Opened)
            {
                await TellClientErrorAsync(context, "Cannot take non-opened task.");
                return;
            }
            var executor = await db.Profiles.FindAsync(executorId);
            if (executor == null)
            {
                await TellNotFoundAsync(context, nameof(executor));
                return;
            }
            quest.Executor = executor;
            quest.Status = Quest.QuestStatus.InProgress;
            // TODO: notify.
            await db.SaveChangesAsync();
        }

        public async Task CancelQuestAsync(HttpContext context, Guid questId, long initiatorId)
        {
            using var db = GetDbContext();
            var quest = await db.Quests.FindAsync(questId);
            if (quest == null)
            {
                await TellNotFoundAsync(context, nameof(quest));
                return;
            }
            if (quest.Status == Quest.QuestStatus.Completed)
            {
                await TellClientErrorAsync(context, "Cannot cancel closed task.");
                return;
            }
            if (quest.Customer.Id != initiatorId)
            {
                await TellClientErrorAsync(context, "Cannot cancel foreign task.");
                return;
            }
            quest.Status = Quest.QuestStatus.Cancelled;
            await db.SaveChangesAsync();
        }

        public async Task CompleteQuestAsync(HttpContext context, Guid questId, long customerId)
        {
            using var db = GetDbContext();
            var quest = await db.Quests.FindAsync(questId);
            if (quest == null)
            {
                await TellNotFoundAsync(context, nameof(quest));
                return;
            }
            if (quest.Status != Quest.QuestStatus.InProgress)
            {
                await TellClientErrorAsync(context, "Cannot mark as completed quest that is not in progress.");
                return;
            }
            if (quest.CustomerId != customerId)
            {
                await TellClientErrorAsync(context, "Cannot mark as completed quest that is not owned by you.");
                return;
            }
            var customer = await db.Profiles.FindAsync(customerId);
            if (customer == null)
            {
                await TellNotFoundAsync(context, nameof(customer));
                return;
            }
            var executor = await db.Profiles.FindAsync(quest.ExecutorId);
            if (executor == null)
            {
                await TellNotFoundAsync(context, nameof(executor));
                return;
            }
            executor.CompletedQuests.Add(quest);
            executor.Coins += quest.Price;
            quest.Status = Quest.QuestStatus.Completed;
            await db.SaveChangesAsync();
        }
    }
}
