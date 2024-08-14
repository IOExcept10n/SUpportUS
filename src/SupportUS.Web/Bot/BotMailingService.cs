using SupportUS.Web.Data;
using SupportUS.Web.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SupportUS.Web.Bot
{
    public class BotMailingService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
        public async Task MailMessageText(string msgText)
        {
            await Bot.Client.SendTextMessageAsync(Bot.WorkingChat, msgText);
        }

        public async Task MailMessageQuest(Quest quest)
        {
            var inlineMarkup = new InlineKeyboardMarkup()
                .AddNewRow().AddButton("Взять задание ✔️", "GetQuest");
            var message = await Bot.Client.SendTextMessageAsync(Bot.WorkingChat, BotQuestService.GenerateMessageText(quest), replyMarkup: inlineMarkup);
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            quest.MailMessageId = message.MessageId;
            db.Update(quest);
            await db.SaveChangesAsync();
        }

        public async Task UpdateMessageQuest(Quest quest)
        {
            if (quest.MailMessageId == null)
            {
                return;
            }
            InlineKeyboardMarkup? markup = null;
            if (quest.Status == Quest.QuestStatus.Opened)
            {
                markup = new InlineKeyboardMarkup()
                    .AddNewRow().AddButton("Взять задание ✔️", "GetQuest");
            }
            await Bot.Client.EditMessageTextAsync(Bot.WorkingChat, quest.MailMessageId.Value, BotQuestService.GenerateMessageText(quest), replyMarkup: markup);
        }

        public async Task MailQuestTaken(Quest quest)
        {
            if (quest.ExecutorId == null)
                return;
            var markup = new InlineKeyboardMarkup()
                .AddNewRow().AddButton("✔️ Подтвердить выполнение", "QuestCompleted");
            var user = await Bot.Client.GetChatMemberAsync(quest.ExecutorId, quest.ExecutorId.Value);
            await Bot.Client.SendTextMessageAsync(quest.CustomerId, $"✔️ Пользователь @{user.User.Username} начал выполнение вашей задачи.");
        }

        public async Task MailQuestCompleted(Quest quest)
        {
            if (quest.ExecutorId == null)
                return;
            await Bot.Client.SendTextMessageAsync(quest.ExecutorId.Value, $"✔️ Квест успешно выполнен!");
        }

        private async Task OnCallbackQueryMail(CallbackQuery callbackQuery)
        {
            switch (callbackQuery.Data)
            {
                case "GetQuest":
                    await Bot.QuestService.TakeQuest(callbackQuery.Message, callbackQuery.From);
                    break;

                case "CancledQuest":
                    await Bot.Client.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
                    break;

                case "QuestCompleted":
                    await Bot.QuestService.CompleteQuest(callbackQuery.Message);
                    break;
            }
        }
    }
}