using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Requests;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class StartCommandHandler : IRequestHandler<StartCommandRequest>
    {
        private readonly ITelegramBotClient _botClient;

        public StartCommandHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<Unit> Handle(StartCommandRequest request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            const string whatBotDoes = "Бот попытается угадать то что вы загадали.";

            var inlineKeyboardMarkup = new InlineKeyboardMarkup(
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Начать новую игру!", "new-game")
                });

            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: whatBotDoes,
                                                  replyMarkup: inlineKeyboardMarkup,
                                                  cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
