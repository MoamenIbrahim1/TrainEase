using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        public int TrainId { get; set; }
        public Train Train { get; set; } = null!;
        public int RouteId { get; set; }
        public Route Route { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int SourceStationId { get; set; }
        public Station SourceStation { get; set; } = null!;

        public int DestinationStationId { get; set; }
        public Station DestinationStation { get; set; } = null!;
    }
}