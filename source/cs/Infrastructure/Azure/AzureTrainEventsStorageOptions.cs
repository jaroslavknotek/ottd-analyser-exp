using System.ComponentModel.DataAnnotations;

namespace TrainsPlatform.Infrastructure.Azure
{
    public class AzureTrainEventsStorageOptions
    {
        [Required]
        public string StorageAccountConnectionString { get; set; } = null!;
        [Required]
        public string RawEventsTableName { get; set; } = null!;
    }
}
