using TrainEase.Core.Domain.Common;

namespace TrainEase.Core.Domain.Entities
{
    public class Route : BaseEntity
    {
        public string? RouteName { get; set; }
        public ICollection<RouteStation> RouteStations { get; set; } = new HashSet<RouteStation>();
        public ICollection<Schedule> Schedules { get; set; } = new HashSet<Schedule>();
    }
}
