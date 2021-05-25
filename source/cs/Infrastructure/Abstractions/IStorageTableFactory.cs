namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IStorageTableFactory
    {
        IStorageTable GetClientEventsTable();
    }
}
