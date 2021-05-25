using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.InMemory
{
    public class InMemoryEventHubFactory : IEventHubFactory
    {
        public IEventHub GetClientEventsEventHub() =>new InMemoryEventHub();
    }
}
