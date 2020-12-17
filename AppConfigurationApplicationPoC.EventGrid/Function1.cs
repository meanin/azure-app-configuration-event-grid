// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

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
            var sentinel = _configuration["appsection:sentinel"];
            // Invalidate cached key-values before calling TryRefreshAsync
            _configurationRefresher.SetDirty(TimeSpan.FromSeconds(0));
            await _configurationRefresher.TryRefreshAsync();
            while (sentinel == _configuration["appsection:sentinel"])
            {
                Console.WriteLine(
                    $"Sentinel value is not updated yet: {_configuration["appsection:sentinel"]}");
                await Task.Delay(TimeSpan.FromSeconds(1));
                await _configurationRefresher.TryRefreshAsync();
            }
            var message = new Dictionary<string, string>
            {
                {"appsection:key1", _configuration["appsection:key1"] },
                {"appsection:key2", _configuration.GetValue<string>("appsection:key2") },
                {"appsection:sentinel", _configuration["appsection:sentinel"] }
            };
            Console.WriteLine(JsonConvert.SerializeObject(message));
        }
    }
}
