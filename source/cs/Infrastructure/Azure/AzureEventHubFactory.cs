using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;

using Microsoft.Extensions.Options;

using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.Azure
{
    public class AzureEventHubFactory : IEventHubFactory
    {
        private readonly AzureClientEventHubOptions _clientEventsOptions;

        public AzureEventHubFactory(IOptions<AzureClientEventHubOptions> clientEventOptionsAccessor)
        {
            _clientEventsOptions = clientEventOptionsAccessor.Value;
        }

        public IEventHub GetClientEventsEventHub()
        {
            var consumer = new EventHubConsumerClient(
                _clientEventsOptions.EventProcessingConsumerGroup,
                _clientEventsOptions.ClientEventsListenerEventHubConnectionString);

            var producer = new EventHubProducerClient(
                _clientEventsOptions.ClientEventsSenderEventHubConnectionString,
                _clientEventsOptions.EventHubName);

            return new AzureEventHub(consumer, producer);
        }
    }
}
