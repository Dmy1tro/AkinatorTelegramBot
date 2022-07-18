using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Requests;
using MediatR;
using Telegram.Bot;

namespace Akinator.Api.Handlers
{
    internal class SendTextMessageHandler : IRequestHandler<SendTextMessageRequest>
    {
        private readonly ITelegramBotClient _botClient;

        public SendTextMessageHandler(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task<Unit> Handle(SendTextMessageRequest request, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(request.ChatId, request.Text, cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
