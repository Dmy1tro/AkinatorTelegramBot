using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Requests;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class UsageCommandHandler : IRequestHandler<UsageCommandRequest>
    {
        private readonly ITelegramBotClient _botClient;

        public UsageCommandHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<Unit> Handle(UsageCommandRequest request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            const string usage = "Usage:\n" +
                                 "/start - start new game";

            await _botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: usage,
                                                  replyMarkup: new ReplyKeyboardRemove());

            return Unit.Value;
        }
    }
}
