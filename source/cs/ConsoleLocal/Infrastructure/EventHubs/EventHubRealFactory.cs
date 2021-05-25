using System;
using System.Collections.Concurrent;

using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;

using Microsoft.Extensions.Options;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Models;
using TrainsPlatform.Infrastructure.Abstractions;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
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

        public IEventHubWriterReader<T> GetClientEventsEventHub<T>()
            where T : new()
        {
            if (typeof(T) != typeof(TrainEvent))
            {
                throw new InvalidOperationException("Only for TrainEvents");
            }
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
            return (IEventHubWriterReader<T>) new EventHubWriterReader<TrainEvent>(eventHub);
        }
    }
}
