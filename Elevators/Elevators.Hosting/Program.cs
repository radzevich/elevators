using Elevators.Infrastructure.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Elevators.Hosting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseCustomLogging()
                .UseKestrel();
        }
    }
}