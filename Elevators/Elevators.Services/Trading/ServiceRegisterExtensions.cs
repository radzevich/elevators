using Elevators.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Elevators.Services.Trading
{
    public static class ServiceRegisterExtensions
    {
        public static IServiceCollection AddTradeService(this IServiceCollection services)
        {
            services.AddSingleton<ITradeService, TradeService>();

            return services;
        }
    }
}