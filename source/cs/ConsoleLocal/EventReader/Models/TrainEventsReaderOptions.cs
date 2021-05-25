using System.ComponentModel.DataAnnotations;

namespace TrainsPlatform.ConsoleLocal.Infrastructure.EventReader.Models
{
    public class TrainEventsReaderOptions
    {
        [Required]
        public string EventDirectory { get; set; }
    }
}
