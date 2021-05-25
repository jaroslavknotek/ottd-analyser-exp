using System.Collections.Concurrent;

using TrainsPlatform.Infrastructure.Abstractions;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
    public class EventHubSimulatorFactory : IEventHubFactory
    {
        private readonly ConcurrentDictionary<string, EventHubSimulator> _eventHubs = new();

        public IEventHubWriterReader<T> GetClientEventsEventHub<T>()
            where T : new()
        {
            var eventHub = _eventHubs.GetOrAdd("client-events", (_) => new EventHubSimulator());
            return (IEventHubWriterReader<T>) new EventHubWriterReader<TrainEvent>(eventHub);
        }
    }
}
