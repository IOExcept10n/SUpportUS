using Telegram.Bot;

namespace SupportUS.Web.Bot
{
    public class BotService
    {
        private readonly string token;

        public TelegramBotClient Client { get; }

        public WebApplication Application { get; }

        public BotService(WebApplication app, string token)
        {
            this.token = token;
            Application = app;
            Client = new TelegramBotClient(token);
        }

        public async Task InitializeAsync()
        {
            
        }
    }
}
