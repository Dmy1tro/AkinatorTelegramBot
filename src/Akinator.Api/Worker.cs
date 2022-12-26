using System;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Models;
using Akinator.Api.Requests;
using Akinator.Core.Exceptions;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Akinator.Api
{
    internal class Worker
    {
        private readonly ITelegramBotClient _bot;
        private readonly IMediator _mediator;

        public Worker(ITelegramBotClient bot, IMediator mediator)
        {
            _bot = bot;
            _mediator = mediator;
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
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(update.CallbackQuery!),

                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleError(exception, GetChatId(update), cancellationToken);
            }
        }

        private async Task BotOnMessageReceived(Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            var chatId = message.Chat.Id;

            Task action = message.Text!.Trim() switch
            {
                StartRequest.RequestName => _mediator.Send(new StartRequest(chatId)),
                _ => _mediator.Send(new HowToUseRequest(chatId))
            };

            try
            {
                await action;
            }
            catch (Exception ex)
            {
                PrintErrorToConsole(ex);
            }

            Console.WriteLine($"The message was sent with id: {message.MessageId}");
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            var chatId = callbackQuery.Message.Chat.Id;
            var callbackData = JsonConvert.DeserializeObject<CallbackData>(callbackQuery.Data);

            Task action = callbackData.Request switch
            {
                StartNewGameRequest.RequestName => _mediator.Send(new StartNewGameRequest(chatId)),
                MakeAnswerRequest.RequestName => _mediator.Send(new MakeAnswerRequest(chatId, callbackData))
            };

            try
            {
                await action;
            }
            catch (Exception ex)
            {
                PrintErrorToConsole(ex);
            }
            finally
            {
                await _bot.AnswerCallbackQueryAsync(callbackQuery.Id);
            }

            Console.WriteLine($"The message was sent with id: {callbackQuery.Message?.MessageId}");
        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
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
