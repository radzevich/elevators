using System;
using Elevators.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Elevators.Providers.Exmo.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddExmo(this IServiceCollection services)
        {
            services
                .AddSingleton<ExmoApi>()
                .AddSingleton<ExmoClient>()
                .AddSingleton<IStockExchange, ExmoStockExchange>();

            services.AddHttpClient<ExmoApi>(client =>
            {
                client.BaseAddress = new Uri("https://api.exmo.com/v1/");
            });

            return services;
        }
    }
}