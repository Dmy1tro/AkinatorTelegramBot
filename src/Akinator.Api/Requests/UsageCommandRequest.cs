using MediatR;
using Telegram.Bot.Types;

namespace Akinator.Api.Requests
{
    internal class UsageCommandRequest : IRequest
    {
        public UsageCommandRequest(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
