using System.ComponentModel.DataAnnotations;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.Models
{
    public class TrainEventsReaderOptions
    {
        [Required]
        public string EventDirectory { get; set; }
    }
}
