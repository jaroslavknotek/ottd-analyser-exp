using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions
{
    public interface IEventHubFactory
    {
        EventHubWriterReader<TrainEvent> GetClientEventsEventHub();
    }
}
