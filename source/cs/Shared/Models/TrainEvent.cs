using System;
using System.Text.Json.Serialization;

namespace TrainsPlatform.Shared.Models
{
    public class TrainEvent
    {
        [JsonPropertyName("stationId")]
        public string StationId { get; set; } = null!;

        [JsonPropertyName("stationName")]
        public string StationName { get; set; } = null!;

        [JsonPropertyName("vehicleId")]
        public string VehicleId { get; set; } = null!;

        [JsonPropertyName("unitNumber")]
        public string UnitNumber { get; set; } = null!;

        [JsonPropertyName("datetime")]
        public DateTimeOffset DateTime { get; set; }

        [JsonPropertyName("orderNumberCurrent")]
        public int OrderNumberCurrent { get; set; }

        [JsonPropertyName("orderNumberTotal")]
        public int OrderNumberTotal { get; set; }

        [JsonPropertyName("Type")]
        public string Type { get; set; } = null!;
    }
}
