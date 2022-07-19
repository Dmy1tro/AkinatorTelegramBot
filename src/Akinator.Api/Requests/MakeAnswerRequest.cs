using MediatR;
using Telegram.Bot.Types;

namespace Akinator.Api.Requests
{
    internal class MakeAnswerRequest : IRequest
    {
        public MakeAnswerRequest(CallbackQuery callbackQuery)
        {
            CallbackQuery = callbackQuery;
        }

        public CallbackQuery CallbackQuery { get; }
    }
}
