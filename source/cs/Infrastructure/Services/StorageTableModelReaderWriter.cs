using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventHubs
{
    public class StorageTableModelReaderWriter<T>:IStorageTableRepository<T>
        where T: new()
    {
        private readonly IStorageTable _table;
        private readonly Func<T,ITableEntity> _entityMapper;

        public StorageTableModelReaderWriter(IStorageTable table, Func<T,ITableEntity> entityMapper)
        {
            _table = table;
            _entityMapper = entityMapper;
        }
        
        public async Task WriteBatchAsync(IEnumerable<T> records)
        {
            var entities = records.Select(r=> _entityMapper(r));
            await _table.StoreAsync(entities);
        }
    }

}
