using Newtonsoft.Json;
using Telegram.Bot;

namespace SupportUS.Web.Bot
{
    public class BotService
    {
        private readonly string token;

        public TelegramBotClient Client { get; }

        public WebApplication Application { get; private set; }

        public BotAdminService AdminService { get; private set; }

        public BotChatService ChatService { get; private set; }

        public BotMailingService MailingService { get; private set; }

        public BotQuestService QuestService { get; private set; }

        public long WorkingChat { get; }

        public BotService(BotConfig config)
        {
            token = config.Token;
            WorkingChat = config.WorkingChatId;
            Client = new TelegramBotClient(token);
        }

        public async Task InitializeAsync(WebApplication app)
        {
            Application = app;
            AdminService = new(this, app);
            ChatService = new(this, app);
            QuestService = new(this, app);
            MailingService = new(this, app);
        }
    }
}
