using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using SupportUS.Web.Data;

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
                case { CallbackQuery: { } callbackQuery }:
                    {
                            await Bot.MailingService.OnCallbackQueryMail(callbackQuery);
                            await Bot.QuestService.OnCallbackQuests(callbackQuery);
                        break;
                    }
                default: Console.WriteLine($"Received unhandled update {update.Type}"); break;
            };
        }

        //private async Task OnCallbackQuery(CallbackQuery callbackQuery)
        //{
        //    switch (callbackQuery.Data)
        //    {
        //        case "Canceled":
        //            await Bot.Client.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId);
        //            break;

        //        case "Done":
        //            await Bot.Client.SendTextMessageAsync(callbackQuery.Message!.Chat, "Doneeeeeeeee");
        //            break;
        //    }
        //}

        private async Task OnTextMessage(Message msg)
        {
            Application.Logger.LogInformation("Received text '{text}' in {chat}", msg.Text, msg.Chat);
            if (msg.Text!.StartsWith('/'))
                await OnCommand(msg.Text, string.Empty, msg);
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var customer = db.Profiles.Find(msg.From?.Id);
            if (customer == null)
            {
                return;
            }
            if (customer.CurrentDraftQuest != null && customer.QuestStatus != Models.Profile.CreationQuestStatus.None)
            {
                await Bot.QuestService.EditQuest(msg, db, customer);
            }
            switch (msg.Text)
            {
                case "Проверить статус заданий 👽":
                    //string TaskText = "fssdsdsfsf";
                    //var inlineMarkup = new InlineKeyboardMarkup()
                    //.AddNewRow().AddButton("Подтвердить выполнения задания ✔️", "Done")
                    //.AddNewRow().AddButton("Отменить задание ❌", "Canceled");
                    await Bot.Client.SendTextMessageAsync(msg.Chat, $"На данный момент вы не можете посмотреть статус своих заданий. На вашем счёте {customer.Coins} монет.");
                    break;

                case "Создать задание 🍑":
                    await Bot.QuestService.CreateQuest(msg);
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
                    var response = await Bot.Http.PostAsync($"https://localhost:7158/api/profiles/create?id={msg.From!.Id}", null);
                    if (!response.IsSuccessStatusCode)
                    {
                        await Bot.Client.SendTextMessageAsync(msg.Chat.Id, "Не удалось зарегистрировать пользователя.");
                        return;
                    }
                    break;
            }
        }
    }
}
