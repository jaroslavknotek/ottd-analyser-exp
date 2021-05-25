using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos.Table;

namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IStorageTable
    {
        Task StoreAsync(IEnumerable<ITableEntity> entities);
    }
}
