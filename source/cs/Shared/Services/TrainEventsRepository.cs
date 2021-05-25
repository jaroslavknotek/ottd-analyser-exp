using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrainsPlatform.Entities;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.Services
{
    public class TrainEventsRepository
    {
        private readonly ILogger<TrainEventsRepository> _logger;
        private readonly CloudTable _table;

        private readonly string _tableName;
        public TrainEventsRepository(
            IOptions<StorageOptions> storageOptionsAccessor,
            ILogger<TrainEventsRepository> logger)
        {
            _tableName = storageOptionsAccessor.Value.RawEventsTableName;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageOptionsAccessor.Value.StorageAccountConnectionString);
            }
            catch (Exception)
            {
                throw;
            }

            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            _table = tableClient.GetTableReference(_tableName);
            if (!_table.Exists())
            {
                throw new ArgumentException($"Table {_tableName} does not exist. Please, first create the table");
            }
        }

        public async Task StoreAsync(IEnumerable<TrainEvent> trainEvents)
        {
            if (trainEvents is null)
            {
                throw new ArgumentNullException(nameof(trainEvents));
            }

            var batch = new TableBatchOperation();

            foreach (var trainEventBatch in trainEvents.Batch(100))
            {
                foreach (var trainEvent in trainEventBatch)
                {
                    var entity = TrainEventEntity.FromTrainEvent(trainEvent);
                    batch.Insert(entity);
                }

                try
                {
                    await _table.ExecuteBatchAsync(batch);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unexpected error while saving to {_tableName}: {e}");
                    throw;
                }
            }
        }

        // public async IAsyncEnumerable<TrainEvent> GetTransportsInADayAsync(DateTimeOffset datetime)
        // {
        //     var partitionKey = datetime.ToString(TransportEntity.PartitionKeyFormat);
        //     var filterPk = TableQuery.GenerateFilterCondition(
        //         nameof(TransportEntity.PartitionKey),
        //         QueryComparisons.Equal, partitionKey);

        //     var query = new TableQuery<TransportEntity>().Where(filterPk);
        //     TableContinuationToken? continuationToken = null;
        //     do
        //     {
        //         var page = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
        //         continuationToken = page.ContinuationToken;

        //         foreach (var entity in page.Results)
        //         {
        //             var model = TransportEntity.ToModel(entity);
        //             yield return model;
        //         }
        //     }
        //     while (continuationToken != null);
        // }
    }


}
