using Elevators.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Elevators.Services.CurrencyRates
{
    public static class ServiceRegisterExtensions
    {
        public static IServiceCollection AddCurrencyRatesStore(this IServiceCollection services)
        {
            services.AddSingleton<ICurrencyRatesStore, CurrencyRatesStore>();

            return services;
        }
    }
}