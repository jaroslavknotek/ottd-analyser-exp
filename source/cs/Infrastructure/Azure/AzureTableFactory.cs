using Microsoft.Extensions.Options;

using TrainsPlatform.Infrastructure.Abstractions;

namespace TrainsPlatform.Infrastructure.Azure
{
    public class AzureTableFactory : IStorageTableFactory
    {
        private readonly AzureTrainEventsStorageOptions _azureTrainEventsStorageOptions;

        public AzureTableFactory(
            IOptions<AzureTrainEventsStorageOptions> azureTrainEventsStorageOptionsAccessor)
        {
            _azureTrainEventsStorageOptions = azureTrainEventsStorageOptionsAccessor.Value;
        }

        public IStorageTable GetClientEventsTable()
        {
            return new AzureTable(_azureTrainEventsStorageOptions);
        }
    }
}
