using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrainsPlatform.Infrastructure.Abstractions
{
    public interface IStorageTableRepository<T>
        where T: new()
    {
        Task WriteBatchAsync(IEnumerable<T> records);
    }
}
