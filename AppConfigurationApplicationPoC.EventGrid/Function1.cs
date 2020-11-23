// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Threading.Tasks;

namespace AppConfigurationApplicationPoC.EventGrid
{
    public class Function1
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationRefresher _configurationRefresher;

        public Function1(
            IConfiguration configuration, 
            IConfigurationRefresher configurationRefresher)
        {
            _configuration = configuration;
            _configurationRefresher = configurationRefresher;
        }

        [FunctionName("Function1")]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent)
        {
            Console.WriteLine($"Before refresh: {_configuration["somekey"]}");
            var refreshed = await _configurationRefresher.TryRefreshAsync();
            Console.WriteLine($"Refreshed: {refreshed}");
            Console.WriteLine($"After refresh: {_configuration["somekey"]}");
        }
    }
}
