using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IStorageTableRepository<T>
        where T : class
    {
        Task WriteBatchAsync(
            IEnumerable<T> records,
            CancellationToken cancellationToken = default);

        Task DeleteBatchAsync(
            IEnumerable<T> records,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<T> ReadAllByPartitionKeyAsync(
            string partitionKey,
            CancellationToken cancellationToken = default);
    }
}
