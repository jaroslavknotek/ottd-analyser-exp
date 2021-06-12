using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
    public class StorageTableModelReaderWriter<T> : IStorageTableRepository<T>
        where T : class
    {
        private readonly IStorageTable _table;
        private readonly Func<T, ITableEntity> _entityMapper;

        public StorageTableModelReaderWriter(IStorageTable table, Func<T, ITableEntity> entityMapper)
        {
            _table = table;
            _entityMapper = entityMapper;
        }

        public async Task WriteBatchAsync(
            IEnumerable<T> records,
            CancellationToken cancellationToken = default)
        {
            var entities = records.Select(r => _entityMapper(r));
            await _table.StoreAsync(entities, cancellationToken);
        }

        public async Task DeleteBatchAsync(
           IEnumerable<T> records,
           CancellationToken cancellationToken = default)
        {
            var entities = records.Select(r => _entityMapper(r));
            await _table.DeleteAsync(entities, cancellationToken);
        }

        public async IAsyncEnumerable<T> ReadAllByPartitionKeyAsync(
            string partitionKey,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var item in _table.GetByPartitionKeyAsync(partitionKey, cancellationToken))
            {
                yield return item as T;
            }
        }
    }

}
