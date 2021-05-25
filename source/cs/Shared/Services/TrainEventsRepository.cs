using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using TrainsPlatform.Entities;
using TrainsPlatform.Infrastructure.Abstractions;
using TrainsPlatform.Shared.Factories;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.Services
{
    public class TrainEventsRepository
    {
        private readonly IStorageTableRepository<TrainEvent> _table;

        public TrainEventsRepository(
            InfrastructureFactory factory,
            ILogger<TrainEventsRepository> logger)
        {
            _table = factory.GetClientEventsTable();
        }

        public async Task StoreAsync(IEnumerable<TrainEvent> trainEvents)
        {
            if (trainEvents is null)
            {
                throw new ArgumentNullException(nameof(trainEvents));
            }

            await _table.WriteBatchAsync(trainEvents);
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
