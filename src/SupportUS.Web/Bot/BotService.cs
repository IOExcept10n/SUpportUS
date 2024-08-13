using Newtonsoft.Json;
using Telegram.Bot;

namespace SupportUS.Web.Bot
{
    public class BotService
    {
        private readonly string token;

        public TelegramBotClient Client { get; }

        public WebApplication Application { get; }

        public BotAdminService AdminService { get; }

        public BotChatService ChatService { get; }

        public BotMailingService MailingService { get; }

        public BotQuestService QuestService { get; }

        public long WorkingChat { get; }

        public BotService(WebApplication app, BotConfig config)
        {
            token = config.Token;
            WorkingChat = config.WorkingChatId;
            Application = app;
            Client = new TelegramBotClient(token);
            AdminService = new(this, app);
            ChatService = new(this, app);
            QuestService = new(this, app);
            MailingService = new(this, app);
        }

        public async Task InitializeAsync()
        {
            
        }
    }
}
