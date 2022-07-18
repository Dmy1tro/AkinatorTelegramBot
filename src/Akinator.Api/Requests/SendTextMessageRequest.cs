using MediatR;
using Telegram.Bot.Types;

namespace Akinator.Api.Requests
{
    internal class SendTextMessageRequest : IRequest
    {
        public SendTextMessageRequest(ChatId chatId, string text)
        {
            ChatId = chatId;
            Text = text;
        }

        public string Text { get; }

        public ChatId ChatId { get; }
    }
}
