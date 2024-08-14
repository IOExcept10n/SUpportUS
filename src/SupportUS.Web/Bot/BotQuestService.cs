using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Data;
using SupportUS.Web.Models;
using System.Net.Mail;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SupportUS.Web.Bot
{
    public class BotQuestService(BotService bot, WebApplication app) : TelegramBotServiceBase(bot, app)
    {
        public async Task CreateQuest(Message msg)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await DraftQuestAsync(msg, db);
            if (quest == null)
                return;
            var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow().AddButton("Название 🐣", "QuestName")
            .AddNewRow().AddButton("Описание ✈️", "QuestDescription")
            .AddNewRow().AddButton("Стоимость 💰", "QuestPrice")
            .AddNewRow().AddButton("Местоположение ☂️", "QuestLocation")
            .AddNewRow().AddButton("Длительность 🪐", "QuestDuration")
            .AddNewRow().AddButton("Дэдлайн🔋", "QuestDeadline")
            .AddNewRow().AddButton("Опубликовать квест 💅", "PublishQuest");
            var message = await Bot.Client.SendTextMessageAsync(
                msg.Chat.Id,
                GenerateMessageText(quest),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, replyMarkup: inlineMarkup);
            quest.BotMessageId = message.MessageId;
            db.Update(quest);
            await db.SaveChangesAsync();
        }

        private async Task<Quest?> DraftQuestAsync(Message msg, QuestsDb db)
        {
            var customer = db.Profiles.Find(msg.From?.Id);
            if (customer == null)
            {
                await Bot.Client.SendTextMessageAsync(msg.Chat.Id,
                                                      "Вы не зарегистрированы. Нажмите /start для начала работы.",
                                                      parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                                                      replyParameters: new() { MessageId = msg.MessageId });
                return null;
            }
            if (customer.CurrentDraftQuest != null)
            {
                return await db.Quests.FindAsync(customer.CurrentDraftQuest);
            }
            var quest = new Quest()
            {
                Id = Guid.NewGuid(),
                Customer = customer,
                CustomerId = customer.Id,
            };
            await db.Quests.AddAsync(quest);
            return quest;
        }

        public async Task EditQuest(Message message, QuestsDb db, Profile profile)
        {
            var quest = await db.Quests.FindAsync(profile.CurrentDraftQuest!.Value);
            if (quest == null)
            {
                await Bot.Client.SendTextMessageAsync(message.Chat.Id, "Не удалось найти квест для редактирования.");
                return;
            }
            switch (profile.QuestStatus)
            {
                case Profile.CreationQuestStatus.Name:
                    {
                        quest.Name = message.Text;
                        break;
                    }
                case Profile.CreationQuestStatus.Description:
                    {
                        quest.Description = message.Text;
                        break;
                    }
                case Profile.CreationQuestStatus.Location:
                    {
                        quest.Location = message.Text;
                        break;
                    }
                case Profile.CreationQuestStatus.Price:
                    {
                        if (int.TryParse(message.Text, out int price))
                        {
                            if (profile.Coins < price)
                            {
                                await Bot.Client.SendTextMessageAsync(message.Chat.Id, "У вас недостаточно средств для указания данной цены.");
                                return;
                            }
                            quest.Price = price;
                            break;
                        }
                        else
                        {
                            await Bot.Client.SendTextMessageAsync(message.Chat.Id, "Введите корректное значение цены.");
                            return;
                        }
                    }
                case Profile.CreationQuestStatus.ExpectedDuration:
                    {
                        if (TimeSpan.TryParse(message.Text, out var duration))
                        {
                            quest.ExpectedDuration = duration;
                            break;
                        }
                        else
                        {
                            await Bot.Client.SendTextMessageAsync(message.Chat.Id, "Введите корректную длительность в формате Ч:ММ:СС.");
                            return;
                        }
                    }
                case Profile.CreationQuestStatus.Deadline:
                    {
                        if (DateTime.TryParse(message.Text, out var deadline))
                        {
                            quest.Deadline = deadline;
                            break;
                        }
                        else
                        {
                            await Bot.Client.SendTextMessageAsync(message.Chat.Id, "Введите корректную дату в формате ГГГГ.ММ.ДД чч:мм:сс.");
                            return;
                        }
                    }
            }
            db.Update(quest);
            await db.SaveChangesAsync();
        }

        internal static string GenerateMessageText(Quest quest)
        {
            string state = quest.Status switch
            {
                Quest.QuestStatus.Draft => "📝 **Задание** \\(черновик\\)",
                Quest.QuestStatus.Opened => "📄 **Задание**",
                Quest.QuestStatus.InProgress => "🔄 **Задание** \\(выполняется\\)",
                Quest.QuestStatus.Completed => "✅ **Задание** \\(выполнено\\)",
                Quest.QuestStatus.Cancelled => "❌ **Задание** \\(отменено\\)",
                _ => "📄 **Задание**",
            };
            return @$"
{state}
**Название**: {quest.Name ?? "\\-"},
**Описание**: {quest.Description ?? "\\-"},
**Стоимость**: {quest.Price},
**Местоположение**: {(quest.Location == null ? "\\-" : '#' + quest.Location.Replace(' ', '_'))},
**Длительность**: {quest.ExpectedDuration?.ToString() ?? "\\-"},
**Дедлайн**: {quest.Deadline?.ToString() ?? "\\-"}
";
        }
        private async Task OnCallbackQuests(CallbackQuery callbackQuery)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            switch (callbackQuery.Data)
            {
                case "QuestName":
                    await UpdateProperty(callbackQuery, db, Profile.CreationQuestStatus.Name);
                    break;
                case "QuestDescription":
                    await UpdateProperty(callbackQuery, db, Profile.CreationQuestStatus.Description);
                    break;
                case "QuestPrice":
                    await UpdateProperty(callbackQuery, db, Profile.CreationQuestStatus.Price);
                    break;
                case "QuestLocation":
                    await UpdateProperty(callbackQuery, db, Profile.CreationQuestStatus.Location);
                    break;
                case "QuestDeadline":
                    await UpdateProperty(callbackQuery, db, Profile.CreationQuestStatus.Deadline);
                    break;
                case "QuestDuration":
                    await UpdateProperty(callbackQuery, db, Profile.CreationQuestStatus.ExpectedDuration);
                    break;
                case "QuestPublish":
                    await PublishQuest(callbackQuery.Message);
                    break;
            }
        }

        public async Task TakeQuest(CallbackQuery query)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await db.Quests.FirstOrDefaultAsync(x => x.MailMessageId == query.Message.MessageId);
            if (quest == null)
            {
                await Bot.Client.AnswerCallbackQueryAsync(query.Id, "Квест не найден в базе данных.");
                return;
            }
            if (quest.Status != Quest.QuestStatus.Opened)
            {
                await Bot.Client.AnswerCallbackQueryAsync(query.Id, "Состояние квеста изменилось, не удаётся подписаться.");
                return;
            }
            var executor = await db.Profiles.FindAsync(query.From.Id);
            if (executor == null)
            {
                await Bot.Client.AnswerCallbackQueryAsync(query.Id, "Вы не зарегистрированы. Введите /start для начала работы.");
                return;
            }
            quest.ExecutorId = executor.Id;
            quest.Executor = executor;
            quest.Status = Quest.QuestStatus.InProgress;
            db.Update(quest);
            await db.SaveChangesAsync();
            await Bot.MailingService.UpdateMessageQuest(quest);
            await Bot.MailingService.MailQuestTaken(quest);
        }

        public async Task CompleteQuest(CallbackQuery query)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var quest = await db.Quests.FirstOrDefaultAsync(x => x.MailMessageId == query.Message!.MessageId);
            if (quest == null)
            {
                await Bot.Client.AnswerCallbackQueryAsync(query.Id, "Квест не найден в базе данных.");
                return;
            }
            if (quest.Status != Quest.QuestStatus.InProgress)
            {
                await Bot.Client.AnswerCallbackQueryAsync(query.Id, "Неверное состояние квеста, не удаётся завершить выполнение.");
                return;
            }
            var executor = await db.Profiles.FindAsync(quest.ExecutorId);
            if (executor == null)
            {
                await Bot.Client.AnswerCallbackQueryAsync(query.Id, "Не удалось найти исполнителя в базе данных.");
                return;
            }
            executor.Coins += quest.Price;
            quest.Status = Quest.QuestStatus.Completed;
            db.Update(quest);
            db.Update(executor);
            await db.SaveChangesAsync();
            await Bot.MailingService.UpdateMessageQuest(quest);
        }

        public async Task PublishQuest(Message message)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var customer = db.Profiles.Find(message.From?.Id);
            if (customer == null)
            {
                await Bot.Client.SendTextMessageAsync(message.Chat.Id,
                                                      "Вы не зарегистрированы. Нажмите /start для начала работы.",
                                                      parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2,
                                                      replyParameters: new() { MessageId = message.MessageId });
                return;
            }
            if (customer.CurrentDraftQuest != null)
            { 
                await db.Quests.FindAsync(customer.CurrentDraftQuest);
                return;
            }
            Quest quest = await db.Quests.FindAsync(customer.CurrentDraftQuest);
            if (customer.Coins < quest.Price)
            {

            }
            {
                
            }
            await Bot.MailingService.MailMessageQuest(quest);
        }

        private async Task UpdateProperty(CallbackQuery callbackQuery, QuestsDb db, Profile.CreationQuestStatus status)
        {
            Message message = callbackQuery.Message;
            string text = status switch
            {
                Profile.CreationQuestStatus.Name => "Введите название квеста",
                Profile.CreationQuestStatus.Description => "Введите описание квеста",
                Profile.CreationQuestStatus.Location => "Введите местоположение квеста",
                Profile.CreationQuestStatus.Price => "Введите цену квеста",
                Profile.CreationQuestStatus.ExpectedDuration => "Введите предполагаемую длительность квеста",
                Profile.CreationQuestStatus.Deadline => "Введите дедлайн квеста",
                _ => "Введён неверный статус."
            };
            await Bot.Client.SendTextMessageAsync(callbackQuery.Message!.Chat, text);
            var customer = db.Profiles.Find(message.From?.Id);
            if (customer == null)
            {
                await Bot.Client.AnswerCallbackQueryAsync(callbackQuery.Id, "\"Вы не зарегистрированы. Нажмите /start для начала работы.");
                return;
            }
            if (customer.CurrentDraftQuest != null)
            {
                var quest = await db.Quests.FindAsync(customer.CurrentDraftQuest);
                if (quest == null)
                {
                    await Bot.Client.AnswerCallbackQueryAsync(callbackQuery.Id, "Не удаётся найти квест. Попробуйте ещё раз.");
                    return;
                }
                customer.QuestStatus = status;
                await db.SaveChangesAsync();
            }
        }
    }
}               