namespace SupportUS.Web.Bot
{
    public class TelegramBotServiceBase(BotService bot, WebApplication app)
    {
        public BotService Bot { get; } = bot;

        public WebApplication Application { get; } = app;
    }
}
