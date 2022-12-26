using MediatR;

namespace Akinator.Api.Requests
{
    internal class StartRequest : IRequest
    {
        public const string RequestName = "/start";

        public StartRequest(long chatId)
        {
            ChatId = chatId;
        }

        public long ChatId { get; }
    }
}
