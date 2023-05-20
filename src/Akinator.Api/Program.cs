using System;
using Akinator.Api.Handlers;
using Akinator.Api.Services;
using Akinator.Core;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Akinator.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildProvider().GetRequiredService<Worker>().Start().GetAwaiter().GetResult();
        }

        private static IServiceProvider BuildProvider()
        {
            var sp = new ServiceCollection();

            sp.AddAkinator();
            sp.AddHttpClient("telegram_client")
                .AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(Configuration.BotToken, client));

            sp.AddTransient<ITelegramUpdateHandler, MakeAnswerHandler>()
              .AddTransient<ITelegramUpdateHandler, StartHandler>()
              .AddTransient<ITelegramUpdateHandler, StartNewGameHandler>()
              .AddTransient<ITelegramUpdateHandler, ShowPossibleGuessesHandler>();

            sp.AddSingleton<IGameStorage, GameStorage>();
            sp.AddSingleton<Worker>();

            return sp.BuildServiceProvider();
        }
    }
}
