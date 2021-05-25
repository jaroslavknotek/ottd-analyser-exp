using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
    public class EventHubReal : IEventHub, IAsyncDisposable
    {
        private readonly EventHubConsumerClient _consumer;
        private readonly EventHubProducerClient _producer;

        public EventHubReal(EventHubConsumerClient consumer, EventHubProducerClient producer)
        {
            _consumer = consumer;
            _producer = producer;
        }

        public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var eventData in _consumer.ReadEventsAsync(cancellationToken))
            {
                yield return eventData.Data.EventBody.ToMemory();
            }
        }

        public async Task WriteBatchAsync(IEnumerable<ReadOnlyMemory<byte>> items, CancellationToken cancellationToken)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            using var eventBatch = await _producer.CreateBatchAsync(cancellationToken);

            foreach (var bytes in items)
            {
                if (!eventBatch.TryAdd(new EventData(bytes)))
                {
                    throw new InvalidOperationException(
                        "Could not add item to batch. Make sure that the batch has at most 100 records");
                }
            }

            await _producer.SendAsync(eventBatch, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _producer.DisposeAsync();
            await _consumer.DisposeAsync();
        }


    }
}
