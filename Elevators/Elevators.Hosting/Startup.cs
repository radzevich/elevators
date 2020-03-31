using System.Linq;
using Elevators.Infrastructure.Logging;
using Elevators.Providers.Exmo.Extensions;
using Elevators.Services.CurrencyRates;
using Elevators.Services.Trading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Elevators.Hosting
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            RegisterInfrastructure(services);
            RegisterProviders(services);
            RegisterServices(services);
        }
        
        private void RegisterInfrastructure(IServiceCollection services)
        {
            services
                .AddCustomLogging();
        }
        
        private void RegisterProviders(IServiceCollection services)
        {
            services.AddExmo();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services
                .AddTradeService()
                .AddCurrencyRatesStore();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}