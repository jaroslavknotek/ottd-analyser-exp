using System;
using System.Globalization;
using Microsoft.Azure.Cosmos.Table;
using TrainsPlatform.Shared.Models;

namespace TrainsPlatform.Entities
{
    public class TrainEventEntity : TableEntity
    {
        public const string PartitionKeyFormat = "yyyyMMdd";
        public TrainEventEntity()
        {

        }

        public TrainEventEntity(
        string stationId,
        string stationName,
        string vehicleId,
        string unitNumber,
        DateTimeOffset dateTime,
        int orderNumberCurrent,
        int orderNumberTotal,
        string type)
            : base(vehicleId, dateTime.ToString("s"))
        {
            if (string.IsNullOrEmpty(stationId))
            {
                throw new ArgumentException($"'{nameof(stationId)}' cannot be null or empty.", nameof(stationId));
            }

            if (string.IsNullOrEmpty(stationName))
            {
                throw new ArgumentException($"'{nameof(stationName)}' cannot be null or empty.", nameof(stationName));
            }

            if (string.IsNullOrEmpty(vehicleId))
            {
                throw new ArgumentException($"'{nameof(vehicleId)}' cannot be null or empty.", nameof(vehicleId));
            }

            if (string.IsNullOrEmpty(unitNumber))
            {
                throw new ArgumentException($"'{nameof(unitNumber)}' cannot be null or empty.", nameof(unitNumber));
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException($"'{nameof(type)}' cannot be null or empty.", nameof(type));
            }

            StationId = stationId;
            StationName = stationName;
            VehicleId = vehicleId;
            UnitNumber = unitNumber;
            DateTime = dateTime;
            OrderNumberCurrent = orderNumberCurrent;
            OrderNumberTotal = orderNumberTotal;
            Type = type;
        }

        public string StationId { get; set; } = null!;

        public string StationName { get; set; } = null!;

        public string VehicleId { get; set; } = null!;

        public string UnitNumber { get; set; } = null!;

        public DateTimeOffset DateTime { get; set; }

        public int OrderNumberCurrent { get; set; }

        public int OrderNumberTotal { get; set; }

        public string Type { get; set; } = null!;

        public static TrainEventEntity FromTrainEvent(
            TrainEvent trainEvent)
        {
            if (trainEvent is null)
            {
                throw new ArgumentNullException(nameof(trainEvent));
            }

            return new TrainEventEntity(
                trainEvent.StationId,
                trainEvent.StationName,
                trainEvent.VehicleId,
                trainEvent.UnitNumber,
                trainEvent.DateTime,
                trainEvent.OrderNumberCurrent,
                trainEvent.OrderNumberTotal,
                trainEvent.Type);
        }

        public static TrainEvent ToModel(TrainEventEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new TrainEvent
            {
                DateTime = entity.DateTime,
                OrderNumberCurrent = entity.OrderNumberCurrent,
                OrderNumberTotal = entity.OrderNumberTotal,
                StationId = entity.StationId,
                StationName = entity.StationName,
                Type = entity.Type,
                UnitNumber = entity.UnitNumber,
                VehicleId = entity.VehicleId
            };
        }
    }
}
