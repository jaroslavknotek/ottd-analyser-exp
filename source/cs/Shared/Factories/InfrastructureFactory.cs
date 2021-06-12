using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

using TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs;
using TrainsPlatform.Entities;
using TrainsPlatform.Infrastructure.Abstractions;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.Shared.Factories
{
    public class InfrastructureFactory
    {
        private readonly IEventHubFactory _eventHubFactory;
        private readonly IStorageTableFactory _storageTableFactory;
        private readonly ConcurrentDictionary<string, IEventHub> _eventHubs = new();
        private readonly ConcurrentDictionary<string, IStorageTable> _storageTables = new();

        public InfrastructureFactory(
            IEventHubFactory eventHubFactory,
            IStorageTableFactory storageTableFactory)
        {
            _eventHubFactory = eventHubFactory;
            _storageTableFactory = storageTableFactory;
        }

        public IEventHubWriterReader<TrainEvent> GetClientEventsEventHub()
        {
            var eventHub = _eventHubs.GetOrAdd("client-events", (_) =>
                _eventHubFactory.GetClientEventsEventHub()
            );
            return new EventHubWriterReader<TrainEvent>(eventHub);
        }

        public IStorageTableRepository<TrainEvent> GetClientEventsRepository()
        {
            var table = _storageTables.GetOrAdd(
                "client-events-table",
                (_) => _storageTableFactory.GetClientEventsTable()
            );
            Func<TrainEvent, TrainEventEntity> mapper = TrainEventEntity.FromTrainEvent;
            return new StorageTableModelReaderWriter<TrainEvent>(table, mapper);
        }
    }


}
