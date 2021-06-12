using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IStorageTable
    {
        Task StoreAsync(
            IEnumerable<ITableEntity> entities,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            IEnumerable<ITableEntity> entities,
            CancellationToken cancellationToken = default);
            
        IAsyncEnumerable<ITableEntity> GetByPartitionKeyAsync(
            string partitionKey,
            CancellationToken cancellationToken = default);
    }
}
