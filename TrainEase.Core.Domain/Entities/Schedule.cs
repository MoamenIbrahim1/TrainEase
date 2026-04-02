using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Schedule : BaseAuditableEntity<int>
    {
        public int TrainId { get; set; }
        public virtual Train Train { get; set; } = null!;
        public int RouteId { get; set; }
        public virtual Route Route { get; set; } = null!;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int SourceStationId { get; set; }
        public virtual Station SourceStation { get; set; } = null!;
        public int DestinationStationId { get; set; }
        public virtual Station DestinationStation { get; set; } = null!;
        public decimal Price { get; set; }

    }
}