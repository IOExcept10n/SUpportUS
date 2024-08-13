namespace SupportUS.Web.Bot
{
    public class BotAdminService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
    }
}
