using SupportUS.Web.Data;
using SupportUS.Web.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SupportUS.Web.Bot
{
    public class BotQuestService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
        public async Task CreateQuest(Message msg)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await DraftQuestAsync(msg, db);
            if (quest == null)
                return;
            var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow().AddButton("Название 🐣", "QuestName")
            .AddNewRow().AddButton("Описание ✈️", "QuestDescription")
            .AddNewRow().AddButton("Стоимость 💰", "QuestPrice")
            .AddNewRow().AddButton("Местоположение ☂️", "QuestLocation")
            .AddNewRow().AddButton("Длительность 🪐", "QuestDuration")
            .AddNewRow().AddButton("Дэдлайн🔋", "QuestDeadline")
            .AddNewRow().AddButton("Опубликовать квест 💅", "PublishQuest");
            var message = await Bot.Client.SendTextMessageAsync(
                msg.Chat.Id,
                GenerateMessageText(quest),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, replyMarkup: inlineMarkup);
            quest.BotMessageId = message.MessageId;
            db.Update(quest);
            await db.SaveChangesAsync();
        }

        private async Task<Quest?> DraftQuestAsync(Message msg, QuestsDb db)
        {
            var customer = db.Profiles.Find(msg.From?.Id);
            if (customer == null)
            {
                await Bot.Client.SendTextMessageAsync(msg.Chat.Id,
                                                      "Вы не зарегистрированы. Нажмите /start для начала работы.",
                                                      parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                                                      replyParameters: new() { MessageId = msg.MessageId });
                return null;
            }
            if (customer.CurrentDraftQuest != null)
            {
                return await db.Quests.FindAsync(customer.CurrentDraftQuest);
            }
            var quest = new Quest()
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                CustomerId = customer.Id,
            };
            await db.Quests.AddAsync(quest);
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
                Quest.QuestStatus.Draft => "📝 **Задание** \\(черновик\\)",
                Quest.QuestStatus.Opened => "📄 **Задание**",
                Quest.QuestStatus.InProgress => "🔄 **Задание** \\(выполняется\\)",
                Quest.QuestStatus.Completed => "✅ **Задание** \\(выполнено\\)",
                Quest.QuestStatus.Cancelled => "❌ **Задание** \\(отменено\\)",
                _ => "📄 **Задание**",
            };
            return @$"
{state}
**Название**: {quest.Name ?? "\\-"},
**Описание**: {quest.Description ?? "\\-"},
**Стоимость**: {quest.Price},
**Местоположение**: {quest.Location ?? "\\-"},
**Длительность**: {quest.ExpectedDuration?.ToString() ?? "\\-"},
**Дедлайн**: {quest.Deadline?.ToString() ?? "\\-"}
";
        }
        private async Task OnCallbackQuests(CallbackQuery callbackQuery)
        {
            await Bot.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"You selected {callbackQuery.Data}");
            switch (callbackQuery.Data)
            {
                case "QuestName":
                {
                    Message message = callbackQuery.Message;
                    using var db = Application.Services.GetRequiredService<QuestsDb>();
                    await Bot.Client.SendTextMessageAsync(callbackQuery.Message!.Chat, "Введите название квеста: ");
                    var customer = db.Profiles.Find(message.From?.Id);
                    if (customer == null)
                    {
                        await Bot.Client.AnswerCallbackQueryAsync(callbackQuery.Id, "\"Вы не зарегистрированы. Нажмите /start для начала работы.");
                        return;
                    }
                    if (customer.CurrentDraftQuest != null)
                    {
                        var quest = await db.Quests.FindAsync(customer.CurrentDraftQuest);
                        quest.Name = message.Text;
                            customer.QuestStatus = Profile.CreationQuestStatus.None;
                    }
                    await Bot.Client.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
                    break;
                }       
            }
        }
    }
}               