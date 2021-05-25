using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.InMemory
{
    public class InMemoryTableFactory : IStorageTableFactory
    {
        public IStorageTable GetClientEventsTable() => new InMemoryStorageTable();
    }
}
