using MediatR;

namespace Akinator.Api.Requests
{
    internal class StartNewGameRequest : IRequest
    {
        public const string RequestName = nameof(StartNewGameRequest);

        public StartNewGameRequest(long chatId)
        {
            ChatId = chatId;
        }

        public long ChatId { get; }
    }
}
