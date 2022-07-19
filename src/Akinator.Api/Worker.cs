using System;
using System.Threading;
using System.Threading.Tasks;
using Akinator.Api.Requests;
using Akinator.Core.Exceptions;
using MediatR;
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

            _bot.StartReceiving(updateHandler: HandleUpdate,
                               pollingErrorHandler: HandlePollingError,
                               receiverOptions: new ReceiverOptions
                               {
                                   AllowedUpdates = Array.Empty<UpdateType>()
                               },
                               cancellationToken: cts.Token);

            Console.WriteLine("Startup is done!");

            while (!cts.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
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

            Task action = message.Text!.Trim() switch
            {
                "/start" => _mediator.Send(new StartCommandRequest(message)),
                _ => _mediator.Send(new UsageCommandRequest(message))
            };

            await action;

            Console.WriteLine($"The message was sent with id: {message.MessageId}");
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            Task action = callbackQuery.Data switch
            {
                "new-game" => _mediator.Send(new StartNewGameRequest(callbackQuery)),
                _ when callbackQuery.Data.StartsWith("answer:") => _mediator.Send(new MakeAnswerRequest(callbackQuery)),
                _ => _mediator.Send(new UsageCommandRequest(callbackQuery.Message))
            };

            await action;

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

        private async Task HandleError(Exception exception, ChatId? chatId, CancellationToken cancellationToken)
        {
            PrintErrorToConsole(exception);

            if (chatId == null)
            {
                return;
            }

            var errorMessage = exception switch
            {
                AkinatorException => "Something went wrong. Please, try again. If this error appears again then start a new game by using /start command",
                _ => ""
            };

            if (!string.IsNullOrEmpty(errorMessage))
                await _mediator.Send(new SendTextMessageRequest(chatId, errorMessage));
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
