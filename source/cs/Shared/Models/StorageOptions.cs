using System.ComponentModel.DataAnnotations;

namespace TrainsPlatform.Shared.Models
{
    public class StorageOptions
    {
        [Required]
        public string StorageAccountConnectionString { get; set; } = null!;
        [Required]
        public string RawEventsTableName { get; set; } = null!;
    }
}
