using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Models;
using Akinator.Api.Requests;
using Akinator.Api.Services;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class MakeAnswerHandler : IRequestHandler<MakeAnswerRequest>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly GameStore _games;

        public MakeAnswerHandler(ITelegramBotClient botClient, GameStore games)
        {
            _botClient = botClient;
            _games = games;
        }

        public async Task<Unit> Handle(MakeAnswerRequest request, CancellationToken cancellationToken)
        {
            var chatId = request.ChatId;
            var gameId = chatId.ToString();
            var game = _games.Get(gameId);

            if (game is null)
            {
                // Just do nothing.
                return Unit.Value;
            }

            var answerId = Convert.ToInt32(request.CallbackData.Data);
            await game.Answer(answerId);

            // if can guess then finish game and remove it from store.
            if (game.CanGuess())
            {
                var guessed = await game.Win();

                _games.Remove(gameId);

                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: "I think you've guessed one of these:",
                                                      cancellationToken: cancellationToken);

                for (int i = 0; i < guessed.Count; i++)
                {
                    await _botClient.SendTextMessageAsync(chatId: chatId,
                                                          text: $"{i + 1}) {guessed[i].Name}",
                                                          cancellationToken: cancellationToken);
                }

                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: "Run /start to start a new game!",
                                                      cancellationToken: cancellationToken);
                return Unit.Value;
            }

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
