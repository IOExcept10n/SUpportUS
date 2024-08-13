namespace SupportUS.Web.Bot
{
    public class BotChatService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
    }
}
