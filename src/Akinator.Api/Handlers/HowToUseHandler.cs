using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Requests;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class HowToUseHandler : IRequestHandler<HowToUseRequest>
    {
        private readonly ITelegramBotClient _botClient;

        public HowToUseHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<Unit> Handle(HowToUseRequest request, CancellationToken cancellationToken)
        {
            const string usage = "Usage:\n" +
                                 "/start - start new game";

            await _botClient.SendTextMessageAsync(chatId: request.ChatId,
                                                  text: usage,
                                                  replyMarkup: new ReplyKeyboardRemove());

            return Unit.Value;
        }
    }
}
