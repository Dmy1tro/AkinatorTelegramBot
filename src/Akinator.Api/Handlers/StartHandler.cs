using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Models;
using Akinator.Api.Requests;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class StartHandler : IRequestHandler<StartRequest>
    {
        private readonly ITelegramBotClient _botClient;

        public StartHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<Unit> Handle(StartRequest request, CancellationToken cancellationToken)
        {
            const string whatBotDoes = "The bot will try to guess what you defined.";

            var inlineKeyboardMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Start a new game!", JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = StartNewGameRequest.RequestName
                    }))
                });

            await _botClient.SendTextMessageAsync(chatId: request.ChatId,
                                                  text: whatBotDoes,
                                                  replyMarkup: inlineKeyboardMarkup,
                                                  cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
