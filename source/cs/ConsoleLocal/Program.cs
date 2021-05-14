using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using System.Text;
using Azure.Messaging.EventHubs.Consumer;
using System.Threading;

namespace ConsoleLocal
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

                   services.Configure<EventHubConfiguration>(context.Configuration);
               });

            var built = host.Build();


            var ehWriterReader = built.Services.GetRequiredService<EhWriterReader>();
            await ehWriterReader.ProduceAsync();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            try
            {
                await ehWriterReader.ReadAsync(cts.Token);
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested) { }
        }
    }

    class EventHubConfiguration
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

        public EhWriterReader(IOptions<EventHubConfiguration> ehOptionsAccessor)
        {
            _consumerConnectionString = ehOptionsAccessor.Value.ClientEventsListenerEventHubConnectionString;
            _producerConnectionString = ehOptionsAccessor.Value.ClientEventsSenderEventHubConnectionString;
            _eventHubName = ehOptionsAccessor.Value.EventHubName;
        }

        public async Task ReadAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventHubConsumerClient(
                EventHubConsumerClient.DefaultConsumerGroupName,
                _consumerConnectionString);

            await foreach (var eventData in consumer.ReadEventsAsync(cancellationToken))
            {
                Console.WriteLine(Encoding.UTF8.GetString(eventData.Data.EventBody));
            }
        }
        public async Task ProduceAsync()
        {
            await using var producerClient = new EventHubProducerClient(_producerConnectionString, _eventHubName);
            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("First event")));
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Second event")));
            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes("Third event")));

            // Use the producer client to send the batch of events to the event hub
            await producerClient.SendAsync(eventBatch);
        }
    }
}
