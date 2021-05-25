using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs.Abstractions
{
    public interface IEventHub
    {
        Task WriteBatchAsync(IEnumerable<ReadOnlyMemory<byte>> items, CancellationToken cancellationToken);
        IAsyncEnumerable<ReadOnlyMemory<byte>> ReadAsync(CancellationToken cancellationToken);
    }
}
