using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
    public class EventHubSimulator : IEventHub
    {
        private readonly Channel<ReadOnlyMemory<byte>> _channel = Channel.CreateBounded<ReadOnlyMemory<byte>>(100);
        public async Task WriteBatchAsync(IEnumerable<ReadOnlyMemory<byte>> items, CancellationToken cancellationToken)
        {
            foreach (var item in items)
            {
                await _channel.Writer.WriteAsync(item, cancellationToken);
            }
        }

        public async IAsyncEnumerable<ReadOnlyMemory<byte>> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return item;
            }
        }

    }
}
