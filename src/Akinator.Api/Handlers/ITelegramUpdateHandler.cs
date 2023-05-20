using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Akinator.Api.Handlers
{
    internal interface ITelegramUpdateHandler
    {
        bool Support(Update update);

        Task Handle(Update update, CancellationToken cancellation = default);
    }
}
