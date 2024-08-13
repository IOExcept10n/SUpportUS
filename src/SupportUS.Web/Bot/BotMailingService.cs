using Telegram.Bot.Types;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using SupportUS.Web.Models;
namespace SupportUS.Web.Bot
{
    public class BotMailingService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
        public async Task MailMessageText(string msgText)
        {
            ChatId chatId = "-1002158004156";
            await Bot.Client.SendTextMessageAsync(chatId, msgText);
        }

        public async Task MailMessageQuest(Quest quest)
        {
            var inlineMarkup = new InlineKeyboardMarkup()
                .AddNewRow().AddButton("Взять задание ✔️", "GetQuest")
                .AddNewRow().AddButton("Скрыть задание ❌", "CanceledQuest");
            await Bot.Client.SendTextMessageAsync(Bot.WorkingChat, BotQuestService.GenerateMessageText(quest), replyMarkup: inlineMarkup);
        }

        private async Task OnCallbackQueryMail(CallbackQuery callbackQuery)
        {
            await Bot.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"You selected {callbackQuery.Data}");
            switch (callbackQuery.Data)
            {
                case "GetQuest":
                    await Bot.Client.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
                    //
                    break;

                case "CancledQuest":
                    await Bot.Client.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
                    break;
            }
        }
    }
}
