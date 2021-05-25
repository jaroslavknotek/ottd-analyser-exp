
using System;
using System.Collections.Generic;
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

        public async Task StoreAsync(IEnumerable<ITableEntity> entities)
        {
            var batchOperation = new TableBatchOperation();

            foreach (var batch in entities.Batch(100))
            {
                foreach (var entity in batch)
                {
                    batchOperation.InsertOrReplace(entity);
                }
                
                await _table.ExecuteBatchAsync(batchOperation);
            }
        }
    }
}
