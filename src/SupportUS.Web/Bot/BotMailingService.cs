namespace SupportUS.Web.Bot
{
    public class BotMailingService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
    }
}
