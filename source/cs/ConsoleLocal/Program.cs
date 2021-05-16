using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using System.Text;
using Azure.Messaging.EventHubs.Consumer;
using System.Threading;
using TrainsPlatform.Shared.Models;
using System.Text.Json;
using TrainsPlatform.Services;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using TrainsPlatform.Services;

namespace TrainsPlatform.ConsoleLocal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host
                .CreateDefaultBuilder(args)
#if DEBUG
                .UseEnvironment("Development")
#endif
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<EhWriterReader>();

                    services.Configure<EventHubOptions>(context.Configuration);
                    services.AddTrainPlatformShared(context.Configuration);

                });

            var built = host.Build();

            var ehWriterReader = built.Services.GetRequiredService<EhWriterReader>();
            var repo = built.Services.GetRequiredService<TrainEventsRepository>();
            await ehWriterReader.ProduceAsync();
            //await ReadMessagesAsync(ehWriterReader, repo);
        }

        private static async Task ReadMessagesAsync(
            EhWriterReader ehWriterReader, 
            TrainEventsRepository repo)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            var list = new List<TrainEvent>();
            try
            {
                await foreach (var trainEvent in ehWriterReader.ReadAsync(cts.Token))
                {
                    list.Add(trainEvent);
                }
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested) { }
            await repo.StoreAsync(list);
        }
    }

    class EventHubOptions
    {
        public string EventHubName { get; set; }
        public string ClientEventsSenderEventHubConnectionString { get; set; }

        public string ClientEventsListenerEventHubConnectionString { get; set; }
    }
    class EhWriterReader
    {
        private readonly string _consumerConnectionString;
        private readonly string _producerConnectionString;
        private readonly string _eventHubName;

        public EhWriterReader(IOptions<EventHubOptions> ehOptionsAccessor)
        {
            _consumerConnectionString = ehOptionsAccessor.Value.ClientEventsListenerEventHubConnectionString;
            _producerConnectionString = ehOptionsAccessor.Value.ClientEventsSenderEventHubConnectionString;
            _eventHubName = ehOptionsAccessor.Value.EventHubName;
        }

        public async IAsyncEnumerable<TrainEvent> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var consumer = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
                _consumerConnectionString);

            await foreach (var eventData in consumer.ReadEventsAsync(cancellationToken))
            {
                var json = Encoding.UTF8.GetString(eventData.Data.EventBody);
                Console.WriteLine(json);

                TrainEvent? trainEvent = null;
                try
                {
                    trainEvent = JsonSerializer.Deserialize<TrainEvent>(json);
                }
                catch (JsonException)
                {
#warning testing only
                }
                if (trainEvent is not null)
                { yield return trainEvent; }
            }
        }
        public async Task ProduceAsync()
        {
            await using var producerClient = new EventHubProducerClient(_producerConnectionString, _eventHubName);
            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
            var trainEvent = new TrainEvent
            {
                DateTime = DateTimeOffset.Now,
                OrderNumberCurrent = 1,
                OrderNumberTotal = 2,
                StationId = "test_station_id",
                StationName = "test_station_name",
                Type = "arrived",
                UnitNumber = "a",
                VehicleId = "veh_1"
            };
            var json = JsonSerializer.Serialize(trainEvent);
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(json)));

            // Use the producer client to send the batch of events to the event hub
            await producerClient.SendAsync(eventBatch);
        }
    }
}
