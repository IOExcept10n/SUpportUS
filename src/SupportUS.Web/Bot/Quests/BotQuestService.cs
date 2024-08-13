using Telegram.Bot.Types;

namespace SupportUS.Web.Bot.Quests
{
    public class BotQuestService
    {
        public async Task CreateQuest(Message msg): BotService
        {
            await bot.SendTextMessageAsync(msg.Chat, TaskText, replyMarkup: inlineMarkup);
        }
    }
}
