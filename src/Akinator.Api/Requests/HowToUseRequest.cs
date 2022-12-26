using MediatR;

namespace Akinator.Api.Requests
{
    internal class HowToUseRequest : IRequest
    {
        public HowToUseRequest(long chatId)
        {
            ChatId = chatId;
        }

        public long ChatId { get; }
    }
}
