using System.Collections.Concurrent;

using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;

using Microsoft.Extensions.Options;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions;
using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Models;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
    public class EventHubSimulatorFactory : IEventHubFactory
    {
        private readonly ConcurrentDictionary<string, EventHubSimulator> _eventHubs = new();

        public EventHubWriterReader<TrainEvent> GetClientEventsEventHub()
        {
            var eventHub = _eventHubs.GetOrAdd("client-events", (_) => new EventHubSimulator());
            return new EventHubWriterReader<TrainEvent>(eventHub);
        }
    }

    public class EventHubRealFactory : IEventHubFactory
    {
        private record ParameterGroup(
            string EventHubName,
            string ConsumerConnectionString,
            string ProducerConnectionString,
            string ConsumerGroup);
        private readonly ParameterGroup _clientEventsGroup;

        public EventHubRealFactory(IOptions<EventHubOptions> ehOptionsAccessor)
        {
            _clientEventsGroup = new ParameterGroup(
                ehOptionsAccessor.Value.EventHubName,
                ehOptionsAccessor.Value.ClientEventsListenerEventHubConnectionString,
                ehOptionsAccessor.Value.ClientEventsSenderEventHubConnectionString,
                ehOptionsAccessor.Value.EventProcessingConsumerGroup);
        }

        private readonly ConcurrentDictionary<string, EventHubReal> _eventHubs = new();

        public EventHubWriterReader<TrainEvent> GetClientEventsEventHub()
        {
            var eventHub = _eventHubs.GetOrAdd(_clientEventsGroup.EventHubName, (_) =>
            {
                var consumer = new EventHubConsumerClient(
                    _clientEventsGroup.ConsumerGroup,
                    _clientEventsGroup.ConsumerConnectionString);

                var producer = new EventHubProducerClient(
                    _clientEventsGroup.ProducerConnectionString,
                    _clientEventsGroup.EventHubName);

                return new EventHubReal(consumer, producer);
            });
            return new EventHubWriterReader<TrainEvent>(eventHub);
        }
    }
}
