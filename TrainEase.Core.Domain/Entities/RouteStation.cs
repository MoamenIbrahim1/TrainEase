using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class RouteStation : BaseEntity
    {
        public int RouteId { get; set; }
        public Route Route { get; set; } = null!;
        public int StationId { get; set; }
        public Station Station { get; set; } = null!;
    }
}