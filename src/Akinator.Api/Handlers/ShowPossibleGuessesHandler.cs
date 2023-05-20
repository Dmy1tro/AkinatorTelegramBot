using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Models;
using Akinator.Api.Services;
using Akinator.Core.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Akinator.Api.Handlers
{
    internal class ShowPossibleGuessesHandler : ITelegramUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IAkinatorClient _akinatorClient;
        private readonly IGameStorage _games;

        public ShowPossibleGuessesHandler(ITelegramBotClient botClient, IAkinatorClient akinatorClient, IGameStorage games)
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

            return callbackData.Request == CallbackData.ShowPossibleGuesses;
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
            var snapshot = _games.Get(gameId);

            if (snapshot is null)
            {
                // Just do nothing.
                return;
            }

            var game = _akinatorClient.LoadGameFromSnapshot(snapshot);
            var guessed = await game.Win();

            await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: "I think you've guessed one of these:",
                                                      cancellationToken: cancellation);

            for (int i = 0; i < guessed.Count; i++)
            {
                await _botClient.SendTextMessageAsync(chatId: chatId,
                                                      text: $"{i + 1}) {guessed[i].Name}",
                                                      cancellationToken: cancellation);
            }
        }
    }
}
