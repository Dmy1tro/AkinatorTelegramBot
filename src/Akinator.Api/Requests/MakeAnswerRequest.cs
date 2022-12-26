using Akinator.Api.Models;
using MediatR;

namespace Akinator.Api.Requests
{
    internal class MakeAnswerRequest : IRequest
    {
        public const string RequestName = nameof(MakeAnswerRequest);

        public MakeAnswerRequest(long chatId, CallbackData callbackData)
        {
            ChatId = chatId;
            CallbackData = callbackData;
        }

        public long ChatId { get; }

        public CallbackData CallbackData { get; }
    }
}
