using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Models;
using Akinator.Api.Requests;
using Akinator.Api.Services;
using Akinator.Core.Interfaces;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class StartNewGameHandler : IRequestHandler<StartNewGameRequest>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAkinatorClient _akinatorClient;
        private readonly GameStore _games;

        public StartNewGameHandler(ITelegramBotClient botClient, IAkinatorClient akinatorClient, GameStore games)
        {
            _botClient = botClient;
            _akinatorClient = akinatorClient;
            _games = games;
        }

        public async Task<Unit> Handle(StartNewGameRequest request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var gameId = chatId.ToString();

            if (_games.Get(gameId) != null)
            {
                _games.Remove(gameId);
            }

            var game = await _akinatorClient.StartNewGame();
            _games.Add(gameId, game);

            var answers = game.GetAnswers();
            var keyboards = new List<List<InlineKeyboardButton>>
            {
                new ()
                {
                    InlineKeyboardButton.WithCallbackData(answers[0].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = MakeAnswerRequest.RequestName,
                        Data = answers[0].Id
                    })),
                    InlineKeyboardButton.WithCallbackData(answers[1].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = MakeAnswerRequest.RequestName,
                        Data = answers[1].Id
                    })),
                },
                new()
                {
                    InlineKeyboardButton.WithCallbackData(answers[2].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = MakeAnswerRequest.RequestName,
                        Data = answers[2].Id
                    })),
                    InlineKeyboardButton.WithCallbackData(answers[3].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = MakeAnswerRequest.RequestName,
                        Data = answers[3].Id
                    })),
                },
                new()
                {
                    InlineKeyboardButton.WithCallbackData(answers[4].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = MakeAnswerRequest.RequestName,
                        Data = answers[4].Id
                    })),
                }
            };

            var inlineKeyboard = new InlineKeyboardMarkup(keyboards);

            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                  text: game.GetQuestion(),
                                                  replyMarkup: inlineKeyboard,
                                                  cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
