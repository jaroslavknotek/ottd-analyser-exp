using System;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using TrainsPlatform.Shared.Models;

namespace AzureFunctions
{
    public class AnomalyDetectionEventHub
    {
        [Function("AnomalyDetectionEventHub")]
        public async Task RunAsync(
            [EventHubTrigger(
                "client-events",
                Connection = "clientEventsListenerEventHubConnectionString",
                ConsumerGroup = "client-events-anomaly-detection-consumer-group")] TrainEvent[] input,
            FunctionContext context)
        {
            var logger = context.GetLogger("AnomalyDetectionEventHub");
            logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
        }
    }
}
