
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.Azure
{
    public class AzureTable : IStorageTable
    {
        private readonly CloudTable _table;

        public AzureTable(AzureTrainEventsStorageOptions options)
        {
            var tableName = options.RawEventsTableName;

            var storageAccount = CloudStorageAccount.Parse(options.StorageAccountConnectionString);

            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var table = tableClient.GetTableReference(tableName);
            if (!table.Exists())
            {
                throw new ArgumentException($"Table {tableName} does not exist. Please, first create the table");
            }

            _table = table;
        }

        public async Task StoreAsync(
            IEnumerable<ITableEntity> entities,
            CancellationToken cancellationToken = default)
        {
            var batchOperation = new TableBatchOperation();

            foreach (var batch in entities.Batch(100))
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var entity in batch)
                {
                    batchOperation.InsertOrReplace(entity);
                }

                await _table.ExecuteBatchAsync(batchOperation, cancellationToken);
            }
        }
        
        public async Task DeleteAsync(
            IEnumerable<ITableEntity> entities,
            CancellationToken cancellationToken = default)
        {
            var batchOperation = new TableBatchOperation();

            foreach (var batch in entities.Batch(100))
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var entity in batch)
                {
                    batchOperation.Add(TableOperation.Delete(entity));
                }

                await _table.ExecuteBatchAsync(batchOperation, cancellationToken);
            }
        }

        public async IAsyncEnumerable<ITableEntity> GetByPartitionKeyAsync(
            string partitionKey,
            [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            var filterPk = TableQuery.GenerateFilterCondition(
                 nameof(ITableEntity.PartitionKey),
                 QueryComparisons.Equal, partitionKey);
            var query = new TableQuery
            {
                FilterString = filterPk
            };
            
            TableContinuationToken token = null;
            do
            {
                var seg = await _table.ExecuteQuerySegmentedAsync(
                    query,
                    token,
                    new TableRequestOptions(),
                    new OperationContext(),
                    cancellationToken);

                token = seg.ContinuationToken;
                foreach (var item in seg)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                }
            } while (token != null);
        }
    }
}

