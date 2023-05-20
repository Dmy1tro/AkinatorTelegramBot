using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class StartHandler : ITelegramUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;

        public StartHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public bool Support(Update update)
        {
            return update.Type == UpdateType.Message &&
                   update.Message!.Type == MessageType.Text &&
                   update.Message.Text?.Trim() == "/start";
        }

        public async Task Handle(Update update, CancellationToken cancellation = default)
        {
            const string whatBotDoes = "The bot will try to define what you have guessed.";

            var inlineKeyboardMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Start a new game!", JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = CallbackData.StartNewGameRequest
                    }))
                });

            await _botClient.SendTextMessageAsync(chatId: update.Message.Chat.Id,
                                                  text: whatBotDoes,
                                                  replyMarkup: inlineKeyboardMarkup,
                                                  cancellationToken: cancellation);
        }
    }
}
