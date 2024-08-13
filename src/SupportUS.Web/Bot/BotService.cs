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

        public BotService(BotConfig config)
        {
            token = config.Token;
            WorkingChat = config.WorkingChatId;
            BotTokenSource = new CancellationTokenSource();
            Client = new TelegramBotClient(token, cancellationToken: BotTokenSource.Token);
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
            Client.OnError += OnError;
            Client.OnMessage += OnMessage;
            Client.OnUpdate += OnUpdate;

            Console.WriteLine($"@{me.Username} is running... Press Escape to terminate");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
            BotTokenSource.Cancel(); // stop the bot


            async Task OnError(Exception exception, HandleErrorSource source)
            {
                Console.WriteLine(exception);
                await Task.Delay(2000, BotTokenSource.Token);
            }

            async Task OnMessage(Message msg, UpdateType type)
            {
                if (string.IsNullOrEmpty(msg.Text))
                {
                    Console.WriteLine($"Received a message of type {msg.Type}");
                }

                else
                {
                    await OnTextMessage(msg);
                }
            }

            async Task OnTextMessage(Message msg) // received a text message that is not a command
            {
                Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");
                if (msg.Text.StartsWith('/'))
                    OnCommand(msg.Text, string.Empty, msg);
                switch (msg.Text)
                {
                    case "Проверить статус заданий 👽":
                        string TaskText = "fssdsdsfsf";
                        var inlineMarkup = new InlineKeyboardMarkup()
                        .AddNewRow().AddButton("Подтвердить выполнения задания ✔️", "Done")
                        .AddNewRow().AddButton("Отменить задание ❌", "Canceled");
                        await Client.SendTextMessageAsync(msg.Chat, TaskText, replyMarkup: inlineMarkup);
                        break;
                }
            }

            async Task OnCommand(string command, string args, Message msg)
            {
                Console.WriteLine($"Received command: {command} {args}");
                switch (command)
                {
                    case "/start":
                        var replyMarkup = new ReplyKeyboardMarkup(true).AddNewRow().AddButton("Проверить статус заданий 👽");
                        await Client.SendTextMessageAsync(msg.Chat, "Keyboard buttons:", replyMarkup: replyMarkup);
                        break;
                }
            }

            async Task OnUpdate(Update update)
            {
                switch (update)
                {
                    case { CallbackQuery: { } callbackQuery }: await OnCallbackQuery(callbackQuery); break;
                    default: Console.WriteLine($"Received unhandled update {update.Type}"); break;
                };
            }

            async Task OnCallbackQuery(CallbackQuery callbackQuery)
            {
                await Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"You selected {callbackQuery.Data}");
                switch (callbackQuery.Data)
                {
                    case "Canceled":
                        await Client.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
                        break;

                    case "Done":
                        await Client.SendTextMessageAsync(callbackQuery.Message!.Chat, "Doneeeeeeeee");
                        break;
                }
            }
        }
    }
}
