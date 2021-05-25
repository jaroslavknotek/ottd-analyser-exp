using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IEventHubFactory
    {
        IEventHubWriterReader<T> GetClientEventsEventHub<T>()
            where T : new();
    }
}
