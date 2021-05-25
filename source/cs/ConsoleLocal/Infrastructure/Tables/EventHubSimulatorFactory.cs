
using Microsoft.Extensions.Options;

using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.Tables
{
    public interface IStorageTableFactory
    {
    }
    public class TableRealFactory : IStorageTableFactory
    {
        public TableRealFactory(
            IOptions<StorageOptions> storageOptionsAccessor)
        {

        }
    }
    public class TableSimulatorFactory : IStorageTableFactory
    {

    }

}
