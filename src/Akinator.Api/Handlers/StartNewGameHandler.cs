using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Requests;
using Akinator.Api.Services;
using Akinator.Core.Interfaces;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class StartNewGameHandler : IRequestHandler<StartNewGameRequest>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAkinatorClient _akinatorClient;
        private readonly GameStore _games;

        public StartNewGameHandler(ITelegramBotClient botClient,
                                           IAkinatorClient akinatorClient,
                                           GameStore games)
        {
            _botClient = botClient;
            _akinatorClient = akinatorClient;
            _games = games;
        }

        public async Task<Unit> Handle(StartNewGameRequest request, CancellationToken cancellationToken)
        {
            var callbackQuery = request.CallbackQuery;
            _games.Remove(callbackQuery.Message.Chat.Id.ToString());

            var game = await _akinatorClient.StartNewGame();
            _games.Add(callbackQuery.Message.Chat.Id.ToString(), game);

            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

            var answers = game.GetAnswers().ToArray();
            var keyboards = new List<List<InlineKeyboardButton>>
            {
                new ()
                {
                    InlineKeyboardButton.WithCallbackData(answers[0], "answer:0"),
                    InlineKeyboardButton.WithCallbackData(answers[1], "answer:1"),
                },
                new()
                {
                    InlineKeyboardButton.WithCallbackData(answers[2], "answer:2"),
                    InlineKeyboardButton.WithCallbackData(answers[3], "answer:3"),
                },
                new()
                {
                    InlineKeyboardButton.WithCallbackData(answers[4], "answer:4"),
                }
            };

            var inlineKeyboard = new InlineKeyboardMarkup(keyboards);

            await _botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id,
                                                  text: game.GetQuestion(),
                                                  replyMarkup: inlineKeyboard,
                                                  cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
