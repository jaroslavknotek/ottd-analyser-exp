using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;

using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.Azure
{
    public class AzureEventHub : IEventHub, IAsyncDisposable
    {
        private readonly EventHubConsumerClient _consumer;
        private readonly EventHubProducerClient _producer;

        public AzureEventHub(EventHubConsumerClient consumer, EventHubProducerClient producer)
        {
            _consumer = consumer;
            _producer = producer;
        }

        public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var parititions = await _consumer.GetPartitionIdsAsync(cancellationToken);

            var channel = Channel.CreateBounded<ReadOnlyMemory<byte>>(100);
            var readertasks = new List<Task>();
            foreach (var paritition in parititions)
            {
                var reader = Task.Run(async () =>
                 {
                     await foreach (var eventData in _consumer.ReadEventsFromPartitionAsync(
                        paritition,
                        EventPosition.FromEnqueuedTime(DateTime.Now.AddMinutes(-5)),
                        cancellationToken))
                     {
                         await channel.Writer.WriteAsync(eventData.Data.EventBody.ToMemory());
                     }
                 }, cancellationToken);
                readertasks.Add(reader);
            }

            await foreach (var item in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return item;
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
