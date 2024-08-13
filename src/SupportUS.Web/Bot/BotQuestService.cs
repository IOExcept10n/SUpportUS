using SupportUS.Web.Data;
using SupportUS.Web.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SupportUS.Web.Bot
{
    public class BotQuestService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
        public async Task CreateQuest(Message msg)
        {
            var quest = await DraftQuestAsync(msg);
            if (quest == null)
                return;
            await Bot.Client.SendTextMessageAsync(
                msg.Chat.Id,
                GenerateMessageText(quest),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
        }

        private async Task<Quest?> DraftQuestAsync(Message msg)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var customer = db.Profiles.Find(msg.From?.Id);
            if (customer == null)
            {
                await Bot.Client.SendTextMessageAsync(msg.Chat.Id,
                                                      "Вы не зарегистрированы. Нажмите /start для начала работы.",
                                                      parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                                                      replyParameters: new() { MessageId = msg.MessageId });
                return null;
            }
            var quest = new Quest()
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                CustomerId = customer.Id
            };
            await db.Quests.AddAsync(quest);
            await db.SaveChangesAsync();
            return quest;
        }

        public async Task EditQuest(Message message)
        {

        }

        public async Task StartEditQuest(Message message)
        {

        }

        internal static string GenerateMessageText(Quest quest)
        {
            string state = quest.Status switch
            {
                Quest.QuestStatus.Draft => "📝 **Задание** (черновик)",
                Quest.QuestStatus.Opened => "📄 **Задание**",
                Quest.QuestStatus.InProgress => "🔄 **Задание** (выполняется)",
                Quest.QuestStatus.Completed => "✅ **Задание** (выполнено)",
                Quest.QuestStatus.Cancelled => "❌ **Задание** (отменено)",
                _ => "📄 **Задание**",
            };
            return @$"
{state}
**Название**: {quest.Name ?? "-"},
**Описание**: {quest.Description ?? "-"},
**Стоимость**: {quest.Price},
**Местоположение**: {quest.Location ?? "-"},
**Длительность**: {quest.ExpectedDuration?.ToString() ?? "-"},
**Дедлайн**: {quest.Deadline?.ToString() ?? "-"}
";
        }
    }
}
