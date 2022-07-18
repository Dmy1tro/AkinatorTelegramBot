using MediatR;
using Telegram.Bot.Types;

namespace Akinator.Api.Requests
{
    internal class StartCommandRequest : IRequest
    {
        public StartCommandRequest(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
