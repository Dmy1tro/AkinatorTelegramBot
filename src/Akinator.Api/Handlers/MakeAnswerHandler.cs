using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Requests;
using Akinator.Api.Services;
using Akinator.Core.Interfaces;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Akinator.Api.Handlers
{
    internal class MakeAnswerHandler : IRequestHandler<MakeAnswerRequest>
    {
        private readonly ITelegramBotClient _botClient;
        private readonly GameStore _games;

        public MakeAnswerHandler(ITelegramBotClient botClient,
                                     GameStore games)
        {
            _botClient = botClient;
            _games = games;
        }

        public async Task<Unit> Handle(MakeAnswerRequest request, CancellationToken cancellationToken)
        {
            var callbackQuery = request.CallbackQuery;
            var game = _games.Get(callbackQuery.Message.Chat.Id.ToString());

            if (game == null)
            {
                await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                return Unit.Value;
            }

            await game.Answer(int.Parse(callbackQuery.Data.Replace("answer:", "")));
            await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

            if (game.CanGuess())
            {
                await MakeGuess(game, callbackQuery, cancellationToken);
                return Unit.Value;
            }

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

        private async Task MakeGuess(IAkinatorGame game, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var guessed = await game.Win();
            var guessedAsArray = guessed.ToArray();

            _games.Remove(callbackQuery.Message.Chat.Id.ToString());

            await _botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id,
                                                  text: "Я думаю что ты загадал что-то из этого:",
                                                  cancellationToken: cancellationToken);

            for (int i = 0; i < guessedAsArray.Length; i++)
            {
                await _botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id,
                                                      text: $"{i + 1}) {guessedAsArray[i].Name}",
                                                      cancellationToken: cancellationToken);
            }

            await _botClient.SendTextMessageAsync(chatId: callbackQuery.Message!.Chat.Id,
                                                  text: "Чтобы начать новую игру выполни комманду /start",
                                                  cancellationToken: cancellationToken);
        }
    }
}
