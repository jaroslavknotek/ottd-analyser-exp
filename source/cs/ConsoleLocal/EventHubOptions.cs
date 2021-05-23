using System.ComponentModel.DataAnnotations;

namespace TrainsPlatform.ConsoleLocal
{
    class EventHubOptions
    {
        [Required]
        public string EventHubName { get; set; } = null!;

        [Required]
        public string ClientEventsSenderEventHubConnectionString { get; set; } = null!;

        [Required]
        public string ClientEventsListenerEventHubConnectionString { get; set; } = null!;

        [Required]
        public string EventProcessingConsumerGroup { get; set; } = null!;
        
        [Required]
        public string AnomaliesConsumerGroup { get; set; }
    }
}
