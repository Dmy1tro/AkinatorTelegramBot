using MediatR;
using Telegram.Bot.Types;

namespace Akinator.Api.Requests
{
    internal class AnswerRequest : IRequest
    {
        public AnswerRequest(CallbackQuery callbackQuery)
        {
            CallbackQuery = callbackQuery;
        }

        public CallbackQuery CallbackQuery { get; }
    }
}
