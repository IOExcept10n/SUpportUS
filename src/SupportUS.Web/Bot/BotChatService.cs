using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;

namespace SupportUS.Web.Bot
{
    public class BotChatService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
        public async Task OnMessage(Message msg, UpdateType type)
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

        public async Task OnError(Exception exception, HandleErrorSource source)
        {
            Console.WriteLine(exception);
            await Task.Delay(2000, Bot.BotTokenSource.Token);
        }



        public async Task OnUpdate(Update update)
        {
            switch (update)
            {
                case { CallbackQuery: { } callbackQuery }: await OnCallbackQuery(callbackQuery); break;
                default: Console.WriteLine($"Received unhandled update {update.Type}"); break;
            };
        }

        private async Task OnCallbackQuery(CallbackQuery callbackQuery)
        {
            await Bot.Client.AnswerCallbackQueryAsync(callbackQuery.Id, $"You selected {callbackQuery.Data}");
            switch (callbackQuery.Data)
            {
                case "Canceled":
                    await Bot.Client.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
                    break;

                case "Done":
                    await Bot.Client.SendTextMessageAsync(callbackQuery.Message!.Chat, "Doneeeeeeeee");
                    break;
            }
        }

        private async Task OnTextMessage(Message msg)
        {
            Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");
            if (msg.Text!.StartsWith('/'))
                await OnCommand(msg.Text, string.Empty, msg);
            switch (msg.Text)
            {
                case "Проверить статус заданий 👽":
                    string TaskText = "fssdsdsfsf";
                    var inlineMarkup = new InlineKeyboardMarkup()
                    .AddNewRow().AddButton("Подтвердить выполнения задания ✔️", "Done")
                    .AddNewRow().AddButton("Отменить задание ❌", "Canceled");
                    await Bot.Client.SendTextMessageAsync(msg.Chat, TaskText, replyMarkup: inlineMarkup);
                    break;

                case "Создать задание 🍑":
                    Bot.QuestService.CreateQuest(msg);
                    break;
            }
        }

        private async Task OnCommand(string command, string args, Message msg)
        {
            Console.WriteLine($"Received command: {command} {args}");
            switch (command)
            {
                case "/start":
                    var replyMarkup = new ReplyKeyboardMarkup(true).AddNewRow().AddButton("Проверить статус заданий 👽").AddButton("Создать задание 🍑");
                    await Bot.Client.SendTextMessageAsync(msg.Chat, "Keyboard buttons:", replyMarkup: replyMarkup);
                    break;
            }
        }
    }
}
