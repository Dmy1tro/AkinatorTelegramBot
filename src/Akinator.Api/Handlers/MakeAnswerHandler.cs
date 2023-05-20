using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Models;
using Akinator.Api.Services;
using Akinator.Core.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class MakeAnswerHandler : ITelegramUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAkinatorClient _akinatorClient;
        private readonly IGameStorage _games;

        public MakeAnswerHandler(ITelegramBotClient botClient, IAkinatorClient akinatorClient, IGameStorage games)
        {
            _botClient = botClient;
            _akinatorClient = akinatorClient;
            _games = games;
        }

        public bool Support(Update update)
        {
            if (update.Type != UpdateType.CallbackQuery)
            {
                return false;
            }

            var callbackData = JsonConvert.DeserializeObject<CallbackData>(update.CallbackQuery.Data);

            return callbackData.Request == CallbackData.MakeAnswerRequest;
        }

        public async Task Handle(Update update, CancellationToken cancellation = default)
        {
            try
            {
                var chatId = update.CallbackQuery.Message.Chat.Id;
                var callbackData = JsonConvert.DeserializeObject<CallbackData>(update.CallbackQuery.Data);

                await Handle(chatId, callbackData, cancellation);
            }
            finally
            {
                await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
        }

        private async Task Handle(long chatId, CallbackData callbackData, CancellationToken cancellation)
        {
            var gameId = chatId.ToString();
            var snapshot = _games.Get(gameId);

            if (snapshot is null)
            {
                // Just do nothing.
                return;
            }

            var game = _akinatorClient.LoadGameFromSnapshot(snapshot);
            var answerId = Convert.ToInt32(callbackData.Data);
            await game.Answer(answerId);
            _games.Save(gameId, game.CreateSnapshot());

            // if can guess then finish game and remove it from store.
            if (game.CanGuess())
            {
                var guessed = await game.Win();

                _games.Remove(gameId);

                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: "I think you've guessed one of these:",
                                                      cancellationToken: cancellation);

                for (int i = 0; i < guessed.Count; i++)
                {
                    await _botClient.SendTextMessageAsync(chatId: chatId,
                                                          text: $"{i + 1}) {guessed[i].Name}",
                                                          cancellationToken: cancellation);
                }

                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: "Run /start to start a new game!",
                                                      cancellationToken: cancellation);

                return;
            }

            var answers = game.GetAnswers();
            var keyboards = new List<List<InlineKeyboardButton>>
            {
                new ()
                {
                    InlineKeyboardButton.WithCallbackData(answers[0].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = CallbackData.MakeAnswerRequest,
                        Data = answers[0].Id
                    })),
                    InlineKeyboardButton.WithCallbackData(answers[1].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = CallbackData.MakeAnswerRequest,
                        Data = answers[1].Id
                    })),
                },
                new()
                {
                    InlineKeyboardButton.WithCallbackData(answers[2].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = CallbackData.MakeAnswerRequest,
                        Data = answers[2].Id
                    })),
                    InlineKeyboardButton.WithCallbackData(answers[3].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = CallbackData.MakeAnswerRequest,
                        Data = answers[3].Id
                    })),
                },
                new()
                {
                    InlineKeyboardButton.WithCallbackData(answers[4].Text, JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = CallbackData.MakeAnswerRequest,
                        Data = answers[4].Id
                    })),
                }
            };

            if (game.GetProgress() > 40)
            {
                keyboards.Add(new()
                {
                    InlineKeyboardButton.WithCallbackData("Show possible guesses", JsonConvert.SerializeObject(new CallbackData
                    {
                        Request = CallbackData.ShowPossibleGuesses
                    }))
                });
            }

            var inlineKeyboard = new InlineKeyboardMarkup(keyboards);

            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                  text: game.GetQuestion(),
                                                  replyMarkup: inlineKeyboard,
                                                  cancellationToken: cancellation);
        }
    }
}
