using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using TrainsPlatform.Services;
using TrainsPlatform.Shared.Models;

namespace AzureFunctions
{
    public class IngressEventHub
    {
        private readonly TrainEventsRepository _trainEventsRepository;

        public IngressEventHub(TrainEventsRepository trainEventsRepository)
        {
            _trainEventsRepository = trainEventsRepository ?? throw new ArgumentNullException(nameof(trainEventsRepository));
        }

        [Function("IngressEventHub")]
        public async Task Run(
            [EventHubTrigger(
                "client-events",
                Connection = "clientEventsListenerEventHubConnectionString",
                ConsumerGroup = "client-events-processing-consumer-group")] TrainEvent[] input,
            FunctionContext context)
        {
            await _trainEventsRepository.StoreAsync(input);

            // calculate regular behavior

            foreach (var trainEvent in input)
            {
                var all = _trainEventsRepository.GetEventsByVehicleAndOrderAsync(
                    trainEvent.VehicleId,
                    trainEvent.OrderNumberCurrent);

                var list = new List<int>();
                await foreach (var e in all)
                {
                    list.Add(e.
                }
            }

            // 


            var logger = context.GetLogger("IngressEventHub");
            logger.LogInformation($"First Event Hubs triggered message: {input[0]}");
        }
    }
}
