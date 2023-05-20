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
    internal class StartNewGameHandler : ITelegramUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAkinatorClient _akinatorClient;
        private readonly IGameStorage _games;

        public StartNewGameHandler(ITelegramBotClient botClient, IAkinatorClient akinatorClient, IGameStorage games)
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

            return callbackData.Request == CallbackData.StartNewGameRequest;
        }

        public async Task Handle(Update update, CancellationToken cancellation = default)
        {
            try
            {
                var chatId = update.CallbackQuery.Message.Chat.Id;

                await Handle(chatId, cancellation);
            }
            finally
            {
                await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            }
        }

        public async Task Handle(long chatId, CancellationToken cancellation)
        {
            var gameId = chatId.ToString();
            var game = await _akinatorClient.StartNewGame();

            _games.Save(gameId, game.CreateSnapshot());

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
                },
            };

            var inlineKeyboard = new InlineKeyboardMarkup(keyboards);

            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                  text: game.GetQuestion(),
                                                  replyMarkup: inlineKeyboard,
                                                  cancellationToken: cancellation);
        }
    }
}
