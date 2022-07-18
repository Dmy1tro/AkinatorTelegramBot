using MediatR;
using Telegram.Bot.Types;

namespace Akinator.Api.Requests
{
    internal class StartNewGameRequest : IRequest
    {
        public StartNewGameRequest(CallbackQuery callbackQuery)
        {
            CallbackQuery = callbackQuery;
        }

        public CallbackQuery CallbackQuery { get; }
    }
}
