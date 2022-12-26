using System;
using Akinator.Core.Interfaces;
using Akinator.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Akinator.Core
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddAkinator(this IServiceCollection services)
        {
            services.AddHttpClient("akinator", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
            }).AddTransientHttpErrorPolicy(builder =>
            {
                return builder.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                });
            });

            services.AddTransient<IAkinatorHttpClient, AkinatorHttpClient>();
            services.AddTransient<IAkinatorClient, AkinatorClient>();

            return services;
        }
    }
}
