using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

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
            _table = factory.GetClientEventsRepository();
        }

        public async Task StoreAsync(IEnumerable<TrainEvent> trainEvents)
        {
            if (trainEvents is null)
            {
                throw new ArgumentNullException(nameof(trainEvents));
            }

            await _table.WriteBatchAsync(trainEvents);
        }

        public async IAsyncEnumerable<TrainEvent> GetEventsByVehicleAndOrderAsync(
            string vehicleId, 
            int orderNumber,
            [EnumeratorCancellation]CancellationToken cancellationToken = default)
        {
            var partitionKey = TrainEventEntity.CreateEntityPartitionKey(vehicleId,orderNumber);
            
            await foreach (var item in _table.ReadAllByPartitionKeyAsync(partitionKey, cancellationToken))
            {
                yield return item;
            }
        }
        
        public async Task DeleteEventsAsync(
            IEnumerable<TrainEvent> events,
            CancellationToken cancellationToken = default)
        {
            await _table.DeleteBatchAsync(events,cancellationToken);
        }
    }


}
