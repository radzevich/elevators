using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

namespace Elevators.Infrastructure.Logging
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddCustomLogging(this IServiceCollection services)
        {
            var timestamp = Stopwatch.GetTimestamp();
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(new JsonFormatter())
                .WriteTo.File(new JsonFormatter(), $"/logs/elevators-{timestamp}.json")
                .CreateLogger();

            return services;
        }

        public static IWebHostBuilder UseCustomLogging(this IWebHostBuilder builder)
        {
            builder.UseSerilog();

            return builder;
        }
    }
}