using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Handlers;
using Akinator.Core.Exceptions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Akinator.Api
{
    internal class Worker
    {
        private readonly ITelegramBotClient _bot;
        private readonly IEnumerable<ITelegramUpdateHandler> _telegramUpdateHandlers;

        public Worker(ITelegramBotClient bot, IEnumerable<ITelegramUpdateHandler> telegramUpdateHandlers)
        {
            _bot = bot;
            _telegramUpdateHandlers = telegramUpdateHandlers;
        }

        public async Task Start()
        {
            var me = await _bot.GetMeAsync();
            Console.Title = me.Username ?? "My awesome Bot";

            using var cts = new CancellationTokenSource();
            var tcs = new TaskCompletionSource();

            _bot.StartReceiving(updateHandler: HandleUpdate,
                               pollingErrorHandler: HandlePollingError,
                               receiverOptions: new ReceiverOptions
                               {
                                   AllowedUpdates = Array.Empty<UpdateType>()
                               },
                               cancellationToken: cts.Token);

            Console.WriteLine("Startup is done!");

            cts.Token.Register(() =>
            {
                tcs.SetResult();
            });

            await tcs.Task;
        }

        private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            foreach (var handler in _telegramUpdateHandlers)
            {
                if (handler.Support(update))
                {
                    try
                    {
                        await handler.Handle(update, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        await HandleError(ex, GetChatId(update), cancellationToken);
                    }
                }
            }
        }

        private ChatId? GetChatId(Update update)
        {
            return update.Type switch
            {
                UpdateType.Message => update.Message.Chat.Id,
                UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat.Id,

                // TODO: add another types if needed
                _ => null
            };
        }

        private Task HandlePollingError(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            PrintErrorToConsole(exception);

            return Task.CompletedTask;
        }

        private async Task HandleError(Exception exception, ChatId chatId, CancellationToken cancellationToken)
        {
            PrintErrorToConsole(exception);

            if (exception is AkinatorException)
            {
                var messageForUser = "Something went wrong. Please, try again. If this error appears again then start a new game by using /start command";

                await _bot.SendTextMessageAsync(chatId, messageForUser, cancellationToken: cancellationToken);
            }
        }

        private void PrintErrorToConsole(Exception exception)
        {
            Console.WriteLine("--------------------------");
            Console.WriteLine(exception.ToString());
            Console.WriteLine(exception.Message);
            Console.WriteLine("--------------------------");
        }
    }
}
