using AppConfigurationApplicationPoC.EventGrid;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AppConfigurationApplicationPoC.EventGrid
{
    public class Startup : FunctionsStartup
    {
        private static IConfiguration Configuration { set; get; }
        private static IConfigurationRefresher ConfigurationRefresher { set; get; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var configurationBuilder = new ConfigurationBuilder()
                .AddAzureAppConfiguration(options =>
                {
                    options.Connect(Environment.GetEnvironmentVariable("AppConfigConnectionString"));
                    options.Select(KeyFilter.Any, "All");

                    ConfigurationRefresher = options.GetRefresher();
                });

            Configuration = configurationBuilder.Build();

            services.AddSingleton(Configuration);
            services.AddSingleton(ConfigurationRefresher);
        }
    }
}