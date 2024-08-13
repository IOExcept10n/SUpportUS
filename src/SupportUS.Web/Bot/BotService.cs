using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

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

        public CancellationTokenSource BotTokenSource { get; }

        public HttpClient Http { get; }

        public BotService(BotConfig config)
        {
            token = config.Token;
            WorkingChat = config.WorkingChatId;
            BotTokenSource = new CancellationTokenSource();
            Http = new HttpClient();
            Client = new TelegramBotClient(token, Http, cancellationToken: BotTokenSource.Token);
        }

        public async Task InitializeAsync(WebApplication app)
        {
            Application = app;
            AdminService = new(this, app);
            ChatService = new(this, app);
            QuestService = new(this, app);
            MailingService = new(this, app);

            var me = await Client.GetMeAsync();
            await Client.DropPendingUpdatesAsync();
            Client.OnError += ChatService.OnError;
            Client.OnMessage += ChatService.OnMessage;
            Client.OnUpdate += ChatService.OnUpdate;

            Console.WriteLine($"Telegram bot@{me.Username} is running... Close backend to terminate...");
        }
    }
}
