namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IEventHubFactory
    {
        IEventHub GetClientEventsEventHub();
    }
}
