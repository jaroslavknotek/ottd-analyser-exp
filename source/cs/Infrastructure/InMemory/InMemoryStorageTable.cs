using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.InMemory
{
    public class InMemoryStorageTable : IStorageTable
    {
        private readonly ConcurrentDictionary<string,ITableEntity> _entities = new();
        public Task StoreAsync(IEnumerable<ITableEntity> entities)
        {
            foreach (var entity in entities)
            {
                var key = $"{entity.PartitionKey}+{entity.RowKey}";
                _entities.AddOrUpdate(key,entity,(k,e)=>entity);
            }
            return Task.CompletedTask;
        }
    }
}
