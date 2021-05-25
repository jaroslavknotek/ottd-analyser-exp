using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IEventHubWriterReader<T> where T : new()
    {
        Task WriteBatchAsync(IEnumerable<T> events, CancellationToken cancellationToken = default);

        IAsyncEnumerable<T> ReadEventsAsync(CancellationToken cancellationToken);
    }
}
