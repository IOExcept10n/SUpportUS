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
                .AddNewRow().AddButton("Взять задание ✔️", "Done")
                .AddNewRow().AddButton("Скрыть задание ❌", "Canceled");
            await Bot.Client.SendTextMessageAsync(Bot.WorkingChat, BotQuestService.GenerateMessageText(quest), replyMarkup: inlineMarkup);
        }
    }
}
